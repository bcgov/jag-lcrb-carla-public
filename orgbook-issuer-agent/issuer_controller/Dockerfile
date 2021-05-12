FROM python:3

ARG HOME="/app/controller"

WORKDIR $HOME

ADD . .

RUN pip3 install --no-cache-dir -r requirements.txt

ENV HOST_IP 0.0.0.0
ENV HOST_PORT 8000
ENV INDY_GENESIS_PATH "${HOME}/genesis"

ENTRYPOINT ["bash", "./docker-entrypoint.sh"]
