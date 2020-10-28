using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestCurve : MonoBehaviour {
    [SerializeField] float sinSpeed = 1.0f;
    [SerializeField] float totalHeight = 10;
    [SerializeField] float segmentBeforShrink = 1.0f;

    float debugCurveDefinition = 1000.0f;

    [SerializeField] Vector2 direction = new Vector2(1.0f, 0.0f);

    void Update() {
        float height = Time.time % totalHeight;
        //Vector2 position = 
        //    new Vector2(
        //        height,
        //        Mathf.Sin(Time.time * sinSpeed * Mathf.PI) * (1 - (height / totalHeight)) + 1
        //        );
        float reduction = Mathf.Clamp((1 + segmentBeforShrink) - (height / (totalHeight - segmentBeforShrink)), 0.0f, 1.0f);
        Vector2 currentMovement = direction * Mathf.Sin(Time.time * sinSpeed * Mathf.PI) * reduction;

        Vector3 position = new Vector3(currentMovement.x, height, currentMovement.y); 

        transform.position = position;
    }

    void OnDrawGizmosSelected() {
        Gizmos.color = Color.red;
        for (float point = 0; point < debugCurveDefinition ; point++) {
            float time = totalHeight / debugCurveDefinition * point;
            float currentLength = time;
            float reduction = Mathf.Clamp((1 + segmentBeforShrink) - (currentLength / (totalHeight - segmentBeforShrink)), 0.0f, 1.0f);
            Vector2 position =
                new Vector2(
                    currentLength,
                    Mathf.Sin(time * sinSpeed * Mathf.PI) * reduction + 1);

            float time2 = totalHeight / debugCurveDefinition * (point + 1);
            float currentLength2 = time2;
            float reduction2 = Mathf.Clamp((1 + segmentBeforShrink) - (currentLength2 / (totalHeight - segmentBeforShrink)), 0.0f, 1.0f);
            Vector2 position2 =
                new Vector2(
                    currentLength2,
                    Mathf.Sin(time2 * sinSpeed * Mathf.PI) * reduction2 + 1);

            Gizmos.DrawLine(position, position2);
        }
    }
}
