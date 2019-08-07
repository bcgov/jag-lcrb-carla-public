FROM splunk/splunk:latest

copy getpass.py /usr/lib/python2.7
copy uid_entrypoint /sbin
RUN chmod 755 /sbin/uid_entrypoint


### user name recognition at runtime w/ an arbitrary uid - for OpenShift deployments
ENTRYPOINT [ "uid_entrypoint" ]