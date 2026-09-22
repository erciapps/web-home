@echo off
cd /d "%~dp0"

if not exist .venv (
    echo Creando entorno virtual...
    py -m venv .venv
)

call .venv\Scripts\activate.bat
python -m pip install --upgrade pip
pip install -r requirements.txt

start "" http://127.0.0.1:8000
mkdocs serve
