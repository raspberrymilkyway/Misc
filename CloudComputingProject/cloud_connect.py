import paramiko
import generator_handler

# paramiko connection thanks to
#  https://stackoverflow.com/questions/55393887/how-to-connect-and-access-google-cloud-compute-engine-vm-via-python-3-6

username = input("Username: ")
hostname = input("\nExternal IP Address: ")

key_filename = input("\nPrivate SSH Key file path: ")

command = input("\nCommand: ")

file_up = input("\nUpload file (y/N)? ")
if file_up.lower() == "yes" or file_up.lower() == "y":
    file_up = input("\nName of file to upload: ")


client = paramiko.SSHClient()
c = client.connect(username=username, hostname=hostname, pkey=key_filename, look_for_keys=False, allow_agent=False)

stdin, stdout, stderr = client.exec_command(command)

print(stdout.read().decode())

client.close()