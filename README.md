# ClassroomAppQuest

This experience is meant to be for an Instructor (in this case a swordsmanship sensei-teacher) to show and share full body motion capture experiences with students, to show them how to properly do techniques and motions. I previously recorded my own full body motion capture using Glycon3D, which is a cheap VR mocap software that anyone can use, which I used for this experience. Other instructors can record their own mocap of doing certain techniques and then use this Classroom experience to share their mocap with other students. Future app features would be to allow instructors to select from a folder the prerecorded mocap animation to share with connected students as well as allowing users to go into an avatar mode and add full body mocap for the users so they can try to mimic the movements while in VR.

Built for Quest/Quest2 Standalone

When starting, users needs to input Name, Region, Email, and choose a Color. All data input is automatically saved to the local machine, so it should only need to be input once. The app allows for 1 Professor and up to 30 students (not tested), alternatively, the user can start the experience in a Debug Mode which spawns the user in as a Professor and spawns 8 Students, to show how the experience would feel with multiple students in the experience. The experience allows for automatic VOIP and will optimize the voices if there are more than 7 students connected by only allowing the 7 loudest students to speak at once (ending voice transmission/networking for the quitest/most silent past the 7 loudest).

As a Student, the user can view the Professors lecture, teleport around the environment, communicate over VOIP, and raise their hand to ask a question.

As a Professor, the user can Mute/Kick/Ban the students, pull up the metadata of individual students, also teleport around the environment, and communicate over VOIP to the students. The Professor can also Move/Rotate/Scale the animated character infront of them to better present the material to the students.

In this experience, the users spawn in as charcter models, using the texture of the color they chose previously, that are connected via an Arc, in the color they chose, to a floating Spirit Orb that is the location of the users HMD. This allows for users to explore the level or change their viewpoint while keeping their Avatars in place infront of the Professor while also showing which HMD is linked with which avatar.
