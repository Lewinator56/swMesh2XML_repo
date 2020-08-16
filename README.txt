swMesh2XML mesh converter instructions

SETUP
------------
- Make sure .net core 3.1 runtime is installled, you can get this off microsoft's website
- extract these file somewhere and run swMesh2XML.exe

PREPARING YOUR MESH
--------------------
- Name your object in blender R-G-B-A/<object name> (where R, G, B, A are color values as integers) (you can leave this blank for a paintable surface)
- Name your material in blender for the object <shaderID>/<material name> (where shaderID is an integer form 0 - 3)
- Triangulate the mesh (ctrl+t)
- Edge split the mesh (you may split on all or hard edges)
- Export as an obj file

IMPORTANT NOTES
----------------
- A paintable vertex must have the color FF 7D 00 FF (make sure your object is named with this suffix to make it paintable)
- each object is treated as a separate submesh

CONVERTING YOUR MESH
---------------------
- Open swMesh2XML and click 'open'
- Switch the file type to '.obj' and browse to your desired object file
- The file will be loaded into the input, you may make any changes here before converting.
- Check the object and material names are in the correct format
- If you want, populate and check the username suffix, this will append your username onto the end of the mesh or phys files
- Click 'to mesh', if everything is valid, the hex for the mesh will be shown in the output
- Click 'save' to choose where to save the mesh.

EXPORTING A PHYS
-----------------
- First export your mesh file, you do not have to save the file, but it mustt be converted to a mesh, the 'gen phys' button will not be anabled until you do this
- Once the mesh is exported, click 'gen Phys', a save dialog will show to choose where to save the physics file

PLEASE REPORT ANY BUGS ON DISCORD