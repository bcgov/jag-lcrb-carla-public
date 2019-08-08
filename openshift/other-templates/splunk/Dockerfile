FROM splunk/splunk:latest

USER root

RUN chmod a+w /etc/passwd

copy uid_entrypoint /sbin

RUN chmod a+w /opt/ansible/ansible.cfg && \
    chmod a+w /opt/ansible/site.yml

copy ansible.cfg /opt/ansible
copy site.yml /opt/ansible

USER ansible

### user name recognition at runtime w/ an arbitrary uid - for OpenShift deployments
ENTRYPOINT [ "uid_entrypoint" ]