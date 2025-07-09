
import socket
import json

def main():
    server_ip = '127.0.0.1'
    server_port = 5000

    try:
        print(f"[INFO] Connecting to gesture server at {server_ip}:{server_port}...")
        sock = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
        sock.connect((server_ip, server_port))
        print("[INFO] Connected to server. Waiting for data...\n")

        while True:
            data = sock.recv(1024)
            if not data:
                print("[INFO] Server closed the connection.")
                break

            try:
                gesture_data = json.loads(data.decode('utf-8'))
                print("[GESTURE DATA]", gesture_data)
            except json.JSONDecodeError:
                print("[ERROR] Failed to decode JSON:", data)

    except ConnectionRefusedError:
        print("[ERROR] Could not connect to the server. Is main.py running?")
    except Exception as e:
        print(f"[ERROR] {e}")
    finally:
        sock.close()

if __name__ == "__main__":
    main()
