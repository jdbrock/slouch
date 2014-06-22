Slouch
======

What is Slouch?
----------------
Slouch is an attempt at a simpler NZB based auto-downloader, inspired by the trio of SABnzbd, Sick Beard and CouchPotato. Like those apps, it will allow you to automatedly download TV episodes and movies. Unlike those apps, it'll be simpler to configure, will have a simpler interface and will simply be one app. Being a single app allows for tighter integration, which will help with tracking successes / failures.

Why?
----------------
I feel that some of the current apps are getting a bit bloated and don't perform very well (especially the intensive web front-end on current versions of CouchPotato). Additionally, the lack of tight integration causes problems with orphaned downloads that have failed and are never retried.

Status
----------------
I'm putting together some of the lower level bits and pieces that are required to fetch things from Usenet. This is mostly complete, and I'll be moving onto the guts of the scheduling, library management, search and metadata components afterwards.
