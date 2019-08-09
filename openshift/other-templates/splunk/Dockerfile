FROM splunk/splunk:latest

USER root

RUN chmod a+w /etc/passwd

copy uid_entrypoint /sbin

RUN chmod -R a+w /opt/ansible && \
  chmod -R 777 /opt/splunk/share/

copy ansible /opt/ansible/

RUN chmod a-w /opt/ansible 

USER ansible

### user name recognition at runtime w/ an arbitrary uid - for OpenShift deployments
ENTRYPOINT [ "uid_entrypoint" ]