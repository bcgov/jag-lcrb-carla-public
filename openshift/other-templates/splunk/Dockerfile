FROM splunk/splunk:latest

copy getpass.py /usr/lib/python2.7
copy uid_entrypoint /sbin


### user name recognition at runtime w/ an arbitrary uid - for OpenShift deployments
ENTRYPOINT [ "uid_entrypoint" ]