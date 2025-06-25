import cv2
import mediapipe as mp
import socket
import threading
import sys
import math
import json

mp_hands = mp.solutions.hands
hands = mp_hands.Hands(
    static_image_mode=False,
    max_num_hands=2,
    min_detection_confidence=0.7,
    min_tracking_confidence=0.7
)
mp_draw = mp.solutions.drawing_utils

left_thumb_start_x = None
right_thumb_start_y = None
left_thumb_x = None
right_thumb_y = None

right_index_finger_extended = False
left_index_finger_extended = False

hand_points = []
frame_width, frame_height = 640, 480
deadzone = 15

def is_index_finger_extended(hand_landmarks):
    # Get required landmarks
    mcp = hand_landmarks.landmark[5]  # base of index
    pip = hand_landmarks.landmark[6]
    dip = hand_landmarks.landmark[7]
    tip = hand_landmarks.landmark[8]
    
    # Calculate distances to determine straightness
    def distance(a, b):
        return math.sqrt((a.x - b.x)**2 + (a.y - b.y)**2 + (a.z - b.z)**2)

    mcp_to_tip = distance(mcp, tip)
    total_segments = distance(mcp, pip) + distance(pip, dip) + distance(dip, tip)

    # If finger is straight, direct mcp→tip distance ~ sum of segments
    straight_ratio = mcp_to_tip / total_segments

    # Threshold for straightness (ideal is ~1.0)
    return straight_ratio > 0.95


def hand_tracking():
    global left_thumb_x, right_thumb_y, left_thumb_start_x, right_thumb_start_y, right_index_finger_extended, left_index_finger_extended
    cap = cv2.VideoCapture(0)
    both_hands = False

    # Socket setup
    server_ip = '127.0.0.1'
    server_port = 5000
    sock = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
    sock.bind((server_ip, server_port))
    sock.listen(1)
    print(f"[INFO] Waiting for client on {server_ip}:{server_port}...")
    conn, addr = sock.accept()
    print(f"[INFO] Connected to {addr}")

    while True:
        success, frame = cap.read()
        if not success:
            break

        frame = cv2.flip(frame, 1)
        rgb_frame = cv2.cvtColor(frame, cv2.COLOR_BGR2RGB)
        results = hands.process(rgb_frame)

        if results.multi_hand_landmarks and len(results.multi_hand_landmarks) > 1:
            both_hands = True
        else:
            both_hands = False

        if results.multi_hand_landmarks and results.multi_handedness:
            for hand_landmarks, hand_type in zip(results.multi_hand_landmarks, results.multi_handedness):
                mp_draw.draw_landmarks(frame, hand_landmarks, mp_hands.HAND_CONNECTIONS)
                hand_label = hand_type.classification[0].label
                
                # index_finger_extended = False
                    
                    
                thumb_tip = hand_landmarks.landmark[4]
                h, w, _ = frame.shape
                cx, cy = int(thumb_tip.x * w), int(thumb_tip.y * h)

                if hand_label == 'Left':
                    left_index_finger_extended = is_index_finger_extended(hand_landmarks)
                    left_thumb_x = cx
                    if left_thumb_start_x is None:
                        left_thumb_start_x = cx
                elif not both_hands:
                    left_thumb_x = None
                    left_thumb_start_x = None

                if hand_label == 'Right':
                    right_index_finger_extended = is_index_finger_extended(hand_landmarks)
                    right_thumb_y = cy
                    if right_thumb_start_y is None:
                        right_thumb_start_y = cy
                elif not both_hands:
                    right_thumb_y = None
                    right_thumb_start_y = None
        else:
            left_thumb_x = None
            left_thumb_start_x = None
            right_thumb_y = None
            right_thumb_start_y = None

        hand_points.clear()
        if results.multi_hand_landmarks:
            for hand_landmarks in results.multi_hand_landmarks:
                for lm in hand_landmarks.landmark:
                    x = int(lm.x * frame_width)
                    y = int(lm.y * frame_height)
                    hand_points.append((x, y))

        # Send gesture data as JSON
        gesture_data = {
            "left_thumb_x": left_thumb_x,
            "left_thumb_start_x": left_thumb_start_x,
            "right_thumb_y": right_thumb_y,
            "right_thumb_start_y": right_thumb_start_y,
            "right_index_extended": bool(right_index_finger_extended),
            "left_index_extended": bool(left_index_finger_extended),
        }

        try:
            conn.sendall(json.dumps(gesture_data).encode('utf-8'))
        except BrokenPipeError:
            print("[ERROR] Client disconnected.")
            break
        except Exception as e:
            print(f"[ERROR] {e}")
            break

        cv2.imshow("Thumb Tracker", frame)
        if cv2.waitKey(1) & 0xFF == ord('q'):
            break

    cap.release()
    cv2.destroyAllWindows()
    conn.close()
    sock.close()
    sys.exit()

hand_tracking() 