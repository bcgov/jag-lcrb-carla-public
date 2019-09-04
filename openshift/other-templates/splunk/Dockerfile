FROM splunk/splunk:latest

USER root

RUN chmod a+w /etc/passwd

copy uid_entrypoint /sbin
#copy entrypoint.sh /sbin


RUN mkdir -p /opt/splunk/etc && \
  mkdir -p /opt/container_artifact && \
  chmod a+w /opt/container_artifact && \
  chmod a+w /opt/splunk/etc && \
  chmod -R a+r /opt/splunk-etc

# copy ansible /opt/ansible/

# RUN chmod a-w /opt/ansible 

RUN chmod -R 777 /opt/splunk

RUN chmod -R a+rx /opt/ansible

USER ansible

RUN /sbin/entrypoint.sh start-and-exit




### user name recognition at runtime w/ an arbitrary uid - for OpenShift deployments
ENTRYPOINT [ "uid_entrypoint" ]