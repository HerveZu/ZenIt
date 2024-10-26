# ZenIt Model Training

## Getting started

Setup a new venv
```shell
python -m venv venv
```

Install requirements
```shell
pip install -r requirements.txt
```

## Train a new model version
Inside your local yolo datasets directory...

```shell
# view yolo settings
yolo settings | grep datasets_dir
```
... add the dataset in the `ZenIt` folder

Optionally, you might want to install torch with cuda if available on your machine.
Then, Run [the train notebook](notebook/train_model.ipynb)