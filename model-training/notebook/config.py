import torch
import os

dir_path = os.path.dirname(os.path.realpath(__file__))

MODEL_PATH = os.path.join(dir_path, "../model/yolov9c/weights/best.pt")
DATA_RECONFIG_PATH = os.path.join(dir_path, "../config.yaml")
YOLO_DEVICE = "cuda" if torch.cuda.is_available() else "cpu"