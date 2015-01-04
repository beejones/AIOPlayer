AIOPlayer
=========

The ultimate All in One Player


People like to put AIO (All In One) PC's or other machines in their office or kitchen. They are used for work, entertainment and
home automation.
Common tasks are viewing camera's, watching TV and controlling the home automation system.

This is exactly for which AIOPlayer is designed.

Home Automation and watching TV channels are very custom tasks. That's why AIOPlayer is full customizable by means of ASX playlists.

AIOPlayer has three customizable sections controlled by four playlists. The directory SamplePlaylists contains samples for each of the 
playlists.

1. master.asx
This is the mean playlist and it points to the location of the other three playlists.
Example:
<ASX version = "3.0">
   <ABSTRACT>All in one player master playlist</ABSTRACT>
   <TITLE>All in one player master playlist</TITLE>
   <AUTHOR>beejones</AUTHOR>
   <ENTRY>
      <TITLE>default</TITLE>
      <REF HREF = ".\SamplePlaylists\default.asx"/>
   </ENTRY>
   <ENTRY>
      <TITLE>channels</TITLE>
      <REF HREF = ".\SamplePlaylists\tv.asx" />
   </ENTRY>
   <ENTRY>
      <TITLE>homecontrol</TITLE>
      <REF HREF =  ".\SamplePlaylists\homecontrol.asx" />
   </ENTRY>
</ASX>

