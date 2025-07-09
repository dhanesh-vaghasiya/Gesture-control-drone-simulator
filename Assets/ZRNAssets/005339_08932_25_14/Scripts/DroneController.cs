using UnityEngine;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System;
using System.Diagnostics;

public class DroneController : MonoBehaviour
{
    public enum ControllerType { KEYBOARD, HAND }
    public ControllerType controllerType;

    public float moveSpeed = 20f;
    public float acceleration = 5f;
    public float ascendSpeed = 8f;
    public float deadzonex = 5f;
    public float deadzoney = 2f;
    public float mouseSensitivity = 2f;
    public float rollSpeed = 90f;
    public float maxRollAngle = 360f;
    public float rotationSmoothness = 5f;
    public float returnSpeed = 2f;

    public Transform droneCamera;
    public float cameraDistance = 6f;
    public float cameraHeight = 2f;
    public float cameraSmoothness = 5f;

    private Rigidbody rb;
    private Vector3 currentVelocity;
    private float pitch, yaw, roll;
    private bool isRolling = false;
    private int rollDirection = 0;
    private float rollAmount = 0f;

    private TcpClient client;
    private NetworkStream stream;
    private Thread receiveThread;
    private bool isRunning = false;

    private float left_thumb_x, left_thumb_start_x;
    private float right_thumb_y, right_thumb_start_y;
    private bool right_index_extended, left_index_extended;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void OnEnable()
    {
        if (controllerType == ControllerType.HAND)
        {
            StartPythonScript();
            ConnectToPython();
        }
    }

    void StartPythonScript()
    {
        string pythonPath = "python"; // Assumes python is added to PATH
        string scriptPath = "Assets/External/Python/main.py"; // Adjust as needed

        ProcessStartInfo startInfo = new ProcessStartInfo
        {
            FileName = pythonPath,
            Arguments = $"\"{scriptPath}\"",
            UseShellExecute = false,
            CreateNoWindow = true
        };

        try
        {
            Process.Start(startInfo);
            UnityEngine.Debug.Log("[Unity] Python script launched.");
        }
        catch (Exception e)
        {
            UnityEngine.Debug.LogError("[Unity] Failed to start Python script: " + e.Message);
        }
    }

    void ConnectToPython()
    {
        int attempts = 0;
        while (client == null && attempts < 10)
        {
            try
            {
                client = new TcpClient("127.0.0.1", 5000);
                stream = client.GetStream();
                isRunning = true;
                receiveThread = new Thread(ReceiveData);
                receiveThread.Start();
                UnityEngine.Debug.Log("[Unity] Connected to Python.");
            }
            catch
            {
                Thread.Sleep(500);
                attempts++;
            }
        }

        if (client == null)
        {
            UnityEngine.Debug.LogError("[Unity] Could not connect to Python after retries.");
        }
    }

    void ReceiveData()
    {
        byte[] buffer = new byte[1024];
        while (isRunning)
        {
            try
            {
                if (stream.DataAvailable)
                {
                    int bytesRead = stream.Read(buffer, 0, buffer.Length);
                    string json = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                    GestureData data = JsonUtility.FromJson<GestureData>(json);

                    left_thumb_x = data.left_thumb_x;
                    left_thumb_start_x = data.left_thumb_start_x;
                    right_thumb_y = data.right_thumb_y;
                    right_thumb_start_y = data.right_thumb_start_y;
                    right_index_extended = data.right_index_extended;
                    left_index_extended = data.left_index_extended;
                }
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogWarning("[Unity] Socket error: " + e.Message);
                isRunning = false;
            }
        }
    }

    void Update()
    {
        float delx = left_thumb_x - left_thumb_start_x;
        float dely = right_thumb_y - right_thumb_start_y;

        if (controllerType == ControllerType.HAND)
        {
            mouseSensitivity = 0.3f;

            if (Mathf.Abs(delx) > deadzonex)
                yaw += delx / 5f * mouseSensitivity;
            if (Mathf.Abs(dely) > deadzoney)
                pitch += dely / 5f * mouseSensitivity;
        }
        else
        {
            mouseSensitivity = 5f;
            yaw += Input.GetAxis("Mouse X") * mouseSensitivity;
            pitch += -Input.GetAxis("Mouse Y") * mouseSensitivity;
        }

        if (Input.GetKey(KeyCode.A)) { isRolling = true; rollDirection = 1; }
        else if (Input.GetKey(KeyCode.D)) { isRolling = true; rollDirection = -1; }
        else { isRolling = false; }

        if (isRolling && Mathf.Abs(rollAmount) < maxRollAngle)
        {
            float delta = rollDirection * rollSpeed * Time.deltaTime;
            roll += delta;
            rollAmount += Mathf.Abs(delta);
        }
        else if (!isRolling && Mathf.Abs(roll) > 0.1f)
        {
            roll = Mathf.Lerp(roll, 0f, Time.deltaTime * returnSpeed);
            if (Mathf.Abs(roll) < 0.5f) { roll = 0f; rollAmount = 0f; }
        }
    }

    void FixedUpdate()
    {
        Quaternion targetRotation = Quaternion.Euler(pitch, yaw, roll);
        rb.MoveRotation(Quaternion.Slerp(rb.rotation, targetRotation, Time.fixedDeltaTime * rotationSmoothness));

        float forwardInput = controllerType == ControllerType.HAND
            ? (right_index_extended ? 1f : left_index_extended ? -1f : 0f)
            : Input.GetKey(KeyCode.W) ? 1f : Input.GetKey(KeyCode.S) ? -1f : 0f;

        float verticalInput = Input.GetKey(KeyCode.Space) ? 1f : Input.GetKey(KeyCode.LeftControl) ? -1f : 0f;

        Vector3 targetVelocity = transform.forward * forwardInput * moveSpeed + transform.up * verticalInput * ascendSpeed;
        currentVelocity = Vector3.Lerp(currentVelocity, targetVelocity, Time.fixedDeltaTime * acceleration);
        rb.linearVelocity = currentVelocity;
    }

    void LateUpdate()
    {
        if (!droneCamera) return;

        Vector3 desiredPos = transform.position - transform.forward * cameraDistance + transform.up * cameraHeight;
        droneCamera.position = Vector3.Lerp(droneCamera.position, desiredPos, Time.deltaTime * cameraSmoothness);
        droneCamera.LookAt(transform.position + transform.forward * 3f);
    }

    void OnApplicationQuit()
    {
        isRunning = false;
        stream?.Close();
        client?.Close();
        receiveThread?.Abort();
    }

    [Serializable]
    public class GestureData
    {
        public float left_thumb_x;
        public float left_thumb_start_x;
        public float right_thumb_y;
        public float right_thumb_start_y;
        public bool right_index_extended;
        public bool left_index_extended;
    }
}
