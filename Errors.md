# swMesh2XML Error Guide
**This file should be used to find out why your mesh caused an error, the following error numbers correspond to the error number displayed in the popup**

Error Number | Cause | Fix
-------------|-------|-----
294 - 300    | Your color on one or more meshes is not formatted correctlly | Make sure the color is in the format R-G-B-A where R,G,B,A are values from 0 to 255
335          | One or more mesh objects have incorrect material names | Make sure that <ul><li> You are exporting materials</li><li> your material is named in the format *shaderID*/*material_name*</li></ul>
312          | One or more mesh objects is incorrectly triangulated | Re-merge all vertices in all mesh objects, then re-triangulate and re-edge-split each mesh object
392          | One or more mesh objects have a vertex with no normals, this normally means there is a line segment in the mesh with no faces | Re-merge all vertices in all mesh objects, then re-triangulate and re-edge-split each mesh object

If your error number is not displayed here, please create a new issue with the error number and your object or blend file so I can take a look.
