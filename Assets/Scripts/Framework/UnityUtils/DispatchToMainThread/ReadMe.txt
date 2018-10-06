目前在Unity3d里使用多线程，没什么特别好的办法使得：
   一段代码可以在Main Thread里运行。
都是使用Monobehaviour里的Update来调用函数运行。

下面是一段示例代码：

//Scale a mesh on a second thread
void ScaleMesh(Mesh mesh, float scale)
{
    //Get the vertices of a mesh
    var vertices = mesh.vertices;
    //Run the action on a new thread
    AsyncTask.RunAsync(()=>{
        //Loop through the vertices
        for(var i = 0; i < vertices.Length; i++)
        {
            //Scale the vertex
            vertices[i] = vertices[i] * scale;
        }
        //Run some code on the main thread
        //to update the mesh
        AsyncTask.QueueOnMainThread(()=>{
            //Set the vertices
            mesh.vertices = vertices;
            //Recalculate the bounds
            mesh.RecalculateBounds();
        });
 
    });
}

-- marked by Allen
