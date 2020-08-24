# swMesh2XML Error Guide
**This file should be used to find out why your mesh caused an error, the following error codes correspond to the error code displayed after the number in the popup**

Error Code | Cause | Fix
-------------|-------|-----
_o           | Your color on one or more meshes is not formatted correctly | Make sure the color is in the format R-G-B-A where R,G,B,A are values from 0 to 255
_usemtl      | One or more mesh objects have incorrect material names | Make sure that <ul><li> You are exporting materials</li><li> your material is named in the format *shaderID*/*material_name*</li></ul>
_v           | One or more mesh objects is incorrectly triangulated | Re-merge all vertices in all mesh objects, then re-triangulate and re-edge-split each mesh object
_vtxDefWrite | One or more mesh objects have a vertex with no normals, this normally means there is a line segment in the mesh with no faces | Re-merge all vertices in all mesh objects, then re-triangulate and re-edge-split each mesh object. If this doesnt work, the following steps should fix it:<ul><li>Locate the problem mesh</li><li>In edit mode, select all faces and run 'merge by distance'</li><li>Then run 'limited dissolve'</li><li>Triangulate the mesh again</li><li>Finally, edge split the mesh again</li></ul>
_vn          | One or more mesh objects have vertices with corrupted or missing normals | Re-merge all verices in all mesh objects, then re-triangulate and re-edge-split each mesh object
_f           | One or more mesh objects have incorrectly defined faces, this is likely the result of a polyline or a quad sneaking into the mesh (somehow) | Re-merge all verices in all mesh objects, then re-triangulate and re-edge-split each mesh
_faceWrite   | For some reason, one or more mesh objects has missing triangles | Re-merge all verices in all mesh objects, then re-triangulate and re-edge-split each mesh
_subWrite    | Something went wrong writing one or more mesh's information (shader, culling) to the file | Re-merge all verices in all mesh objects, then re-triangulate and re-edge-split each mesh


If your error code is not displayed here or the error code is simply a number follwoed by an underscore, please create a new issue with the error number and your object or blend file so I can take a look.
