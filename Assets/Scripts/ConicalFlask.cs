using System.Collections;
using UnityEngine;
using DG.Tweening;

public class ConicalFlask : MonoBehaviour
{
    /// <summary>
    /// Conical flask activity maintain here
    /// </summary>
    public TubeType type;
    private MeshRenderer meshRenderer;
    private BoxCollider coll;
    private Color currentColor;
    private Coroutine colorLerpCoroutine;

    void Start()
    {
        coll = GetComponent<BoxCollider>();
        meshRenderer = transform.GetChild(0).GetComponent<MeshRenderer>();
    }
    // Collider control when required
    public void IsCollider(bool isEnabled)
    {
        if (coll != null)
            coll.enabled = isEnabled;
    }

    // Assign color accordingly
    public void SetColor(Color targetColor, float duration = 1f)
    {
        if (meshRenderer != null)
        {
            if (colorLerpCoroutine != null)
                StopCoroutine(colorLerpCoroutine);

            Invoke(nameof(MixAnimate), 3f);
            colorLerpCoroutine = StartCoroutine(LerpColorCoroutine(targetColor, duration));
        }
    }

    // When solutions are mixing
    private void MixAnimate()
    {
        Vector3 currentRotation = transform.localEulerAngles;

        // First rotate X to 30 while keeping Y and Z the same
        transform.DOLocalRotate(new Vector3(30f, currentRotation.y, currentRotation.z), 0.5f)
            .OnComplete(() =>
            {
                // Then rotate back to X = 0 while keeping Y and Z the same
                transform.DOLocalRotate(new Vector3(0f, currentRotation.y, currentRotation.z), 0.5f);
            });
    }

    // Slowly color change
    private IEnumerator LerpColorCoroutine(Color targetColor, float duration)
    {
        Material[] materials = meshRenderer.materials;
        float time = 0f;

        yield return new WaitForSeconds(2f);
        Manager.Instance.SendMessage(2);
        yield return new WaitForSeconds(2f);
        // Detect color property once per material
        string[] colorProperties = new string[materials.Length];
        Color[] startColors = new Color[materials.Length];

        for (int i = 0; i < materials.Length; i++)
        {
            Material mat = materials[i];
            string property = null;

            if (mat.HasProperty("_Tint"))
                property = "_Tint";
            else if (mat.HasProperty("_TintColor"))
                property = "_TintColor";
            else if (mat.HasProperty("_Color"))
                property = "_Color";
            else if (mat.HasProperty("_BaseColor"))
                property = "_BaseColor";
            else
                Debug.LogWarning($"Material {i} has no recognized color property.");

            colorProperties[i] = property;

            if (property != null)
                startColors[i] = mat.GetColor(property);
        }

        while (time < duration)
        {
            time += Time.deltaTime;

            for (int i = 0; i < materials.Length; i++)
            {
                if (colorProperties[i] == null) continue;

                Color lerpedColor = Color.Lerp(startColors[i], targetColor, time / duration);
                materials[i].SetColor(colorProperties[i], lerpedColor);
            }

            yield return null;
        }

        for (int i = 0; i < materials.Length; i++)
        {
            if (colorProperties[i] == null) continue;

            materials[i].SetColor(colorProperties[i], targetColor);
        }
        CameraController.Instance.ChangeCamera(0);
        int random = Random.Range(0, 2);
        switch (random)
        {
            case 0:
                Manager.Instance.SendMessage(3);
                break;
            case 1:
                Manager.Instance.SendMessage(4);
                break;
        }

        Manager.Instance.SendMessage(5, 3);
    }
}
public enum TubeType
{
    Left, Right
}