# Set up project

## Set up NFC on a phone
It requires [Tasker](https://play.google.com/store/apps/details?id=net.dinglisch.android.taskerm) to be installed on an Android phone. Import the [`tasker_task_tag_1.xml`](NFC/tasker_task_tag_1.xml) as a task in Tasker. Set up a profile that has the NFC scan and activates the task from it.

## Firewall for Unity
When testing in the Unity editor the firewall needs to be disabled for the UDP from the phone to work. For the built project a firewall exception can be added.