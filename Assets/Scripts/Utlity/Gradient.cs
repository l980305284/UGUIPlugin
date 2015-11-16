using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
/*
 * @author 戴佳霖
 * 
 */

[AddComponentMenu("UI/Effects/Gradient")]
public class Gradient : BaseMeshEffect
{
    [SerializeField]
    private Color32 topColor = Color.white;
    [SerializeField]
    private Color32 bottomColor = Color.black;

    public Movement movement = Movement.Vertical;
    public enum Movement
    {
        Horizontal, Vertical,
    }

    public override void ModifyMesh(Mesh mesh)
    {
        if (!IsActive())
        {
            return;
        }

        Vector3[] vertexList = mesh.vertices;
        int count = mesh.vertexCount;
        if (count > 0)
        {
            if (movement == Movement.Vertical)
            {
                float bottomY = vertexList[0].y;
                float topY = vertexList[0].y;

                for (int i = 1; i < count; i++)
                {
                    float y = vertexList[i].y;
                    if (y > topY)
                    {
                        topY = y;
                    }
                    else if (y < bottomY)
                    {
                        bottomY = y;
                    }
                }
                List<Color32> colors = new List<Color32>();
                float uiElementHeight = topY - bottomY;
                for (int i = 0; i < count; i++)
                {
                    colors.Add(Color32.Lerp(bottomColor, topColor, (vertexList[i].y - bottomY) / uiElementHeight));
                }
                mesh.SetColors(colors);
            }
            else
            {
                float bottomX = vertexList[0].x;
                float topX = vertexList[0].x;

                for (int i = 1; i < count; i++)
                {
                    float x = vertexList[i].x;
                    if (x > topX)
                    {
                        topX = x;
                    }
                    else if (x < bottomX)
                    {
                        bottomX = x;
                    }
                }
                List<Color32> colors = new List<Color32>();
                float uiElementHeight = topX - bottomX;
                for (int i = 0; i < count; i++)
                {
                    colors.Add(Color32.Lerp(bottomColor, topColor, (vertexList[i].x - bottomX) / uiElementHeight));
                }
                mesh.SetColors(colors);
            }

        }
    }
}