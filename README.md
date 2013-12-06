ns2_map_stuckspots
==================

This is a tool that uses the annotation system to draw text on your map.
Pretty sweet right?

Basically the majority of annoations are stuck spots in releases since the unstuck from shine generates a note


The tool basically graphs the  stuck spots on the minimaps. one dot per report.


The downside is that I should be reading the  level files to adjust the  scaling of the dots. Becasue I am prety lazy I have not update the library I use to read the latest map versions. this means that you need to manually set the minimap data  in the text file or it just sort of wings it and grpahs the  dots


Uses code from

http://www.codeproject.com/Articles/31702/NET-Targa-Image-Reader

https://github.com/DamienHauta for having a signinficantly better level parser than me.
