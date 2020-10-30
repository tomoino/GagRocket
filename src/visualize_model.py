from keras.models import load_model
from keras.utils import plot_model

def visualize_model(model_path, save_path):
    model = load_model(model_path)
    plot_model(model, to_file=save_path)

visualize_model("model/rnn_model.h5", "data/rnn_model.png")