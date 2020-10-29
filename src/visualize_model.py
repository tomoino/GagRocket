from keras.models import load_model
from keras.utils import plot_model

def visualize_model(model_path, save_path):
    model = load_model(model_path)
    plot_model(model, to_file=save_path)

visualize_model("model/clcnn_model.h5", "data/clcnn_model.png")