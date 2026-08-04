FROM python:3.14-slim
RUN useradd -m flaskuser
RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*   # remove caches

WORKDIR /app
COPY requirements.txt .
RUN pip install -r  requirements.txt
COPY . .
RUN chown -R flaskuser:flaskuser /app
USER flaskuser
RUN   chmod +x entrypoint.sh
ENTRYPOINT ["./entrypoint.sh"]
