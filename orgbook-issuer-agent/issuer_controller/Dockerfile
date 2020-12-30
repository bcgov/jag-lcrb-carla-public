FROM bcgovimages/von-image:py36-1.15-0

WORKDIR $HOME

# `ADD --chown=` is not available in OpenShift yet.
# ADD --chown=indy:indy . $HOME
ADD . $HOME

USER root
RUN apt-get update && apt-get install -y build-essential

RUN pip3 install --upgrade pip setuptools wheel

RUN pip3 install --no-cache-dir -r requirements.txt

USER root

# Add the indy uesr to the root group.
RUN usermod -a -G root indy

ARG entrypoint="${HOME}/docker-entrypoint.sh"

ENV HOST_IP 0.0.0.0
ENV HOST_PORT 8000
ENV INDY_GENESIS_PATH "${HOME}/genesis"

RUN chown -R indy:root "${HOME}" \
    && chmod -R ug+rw "${HOME}" \
    && chmod ug+x "${entrypoint}"

USER indy

# HEALTHCHECK is unnecessary when hosted in OpenShift
# HEALTHCHECK --interval=60s --timeout=5s --start-period=120s \
#     CMD wget -q --spider http://localhost:${HOST_PORT}/health || exit 1

WORKDIR $HOME
ENTRYPOINT ["bash", "./docker-entrypoint.sh"]