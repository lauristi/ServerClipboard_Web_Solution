import requests

url = 'http://192.168.0.156:5020/api/clipboard/get'

try:
    response = requests.get(url)
    response.raise_for_status()  # Lança uma exceção para códigos de erro HTTP

    if response.status_code == 200:
        data = response.json()
        clipboard_data = data.get('clipboard', '')
        print("Clipboard data:", clipboard_data)
    else:
        print(f"Failed to get clipboard data. Status code: {response.status_code}")

except requests.exceptions.RequestException as err:
    print(f"Request Exception: {err}")

