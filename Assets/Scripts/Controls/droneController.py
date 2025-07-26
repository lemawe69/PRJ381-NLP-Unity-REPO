from djitellopy import Tello;
import cv2
import threading
import time

class droneController:
    def __init__(self):
        self.tello = Tello()
        
    def start():
        tello.connect()
        tello.streamon()
        
        thread_video = threading.Thread(target=self.show_video)
        thread_video.start()
        
    def execute_command(cmd):
        match(cmd):
            case "take off":
                tello.takeoff()
            case "land":
                tello.land()
            case "forward":
                tello.move_forward(30)
            case "left":
                tello.move_left(30)
            case "right":
                tello.move_right(30)
            case "up":
                tello.move_up(30)
            case "down":
                tello.move_down(30)
            case _:
                print("Unknown command")
                
    def show_video():
        while True:
            frame = tello.get_frame_read().frame
            _, buffer = cv2.imencode('.jpg', frame)
            yield (b'--frame\r\n'
               b'Content-Type: image/jpeg\r\n\r\n' + buffer.tobytes() + b'\r\n')
            