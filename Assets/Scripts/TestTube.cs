using UnityEngine;
using DG.Tweening;
using System.Collections;
public class TestTube : MonoBehaviour
{
    /// <summary>
    /// Testtube activity handle
    /// </summary>
    private CapsuleCollider coll;
    private Vector3 defPos, offSet, mousePos;
    private MeshRenderer meshRenderer;
    private Color[] sampleColors = new Color[]
 {
    Color.red,
    Color.green,
    Color.blue,
    Color.yellow,
    Color.magenta
 };

    void Start()
    {
        coll = GetComponent<CapsuleCollider>();
        defPos = transform.localPosition;
        meshRenderer = transform.GetChild(0).GetComponent<MeshRenderer>();
    }

    public void IsCollider(bool isEnabled)
    {
        if (coll != null)
            coll.enabled = isEnabled;
    }

    /// <summary>
    /// Drag testtube
    /// </summary>
    void OnMouseDown()
    {
        IsCollider(false);

        Vector3 mousePos = Input.mousePosition;
        mousePos.z = Camera.main.WorldToScreenPoint(transform.position).z; // Set correct Z depth
        offSet = transform.position - Camera.main.ScreenToWorldPoint(mousePos);
    }

    void OnMouseDrag()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = Camera.main.WorldToScreenPoint(transform.position).z; // Keep Z depth
        transform.position = Camera.main.ScreenToWorldPoint(mousePos) + offSet;
    }


    void OnMouseUp()
    {
        bool isMatch = false;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            if (hit.transform.GetComponent<ConicalFlask>())
            {
                ConicalFlask flask = hit.transform.GetComponent<ConicalFlask>();
                if (flask.type == TubeType.Left)
                    CameraController.Instance.ChangeCamera(1);
                else
                    CameraController.Instance.ChangeCamera(2);
                flask.SetColor(GetColorFromMaterial());
                Vector3 targetPos = flask.transform.position + new Vector3(0.4f, 0.5f, 0f);
                StartCoroutine(MatchToFlask(targetPos));
                isMatch = true;
                Invoke(nameof(AutoColorFill), 5f);
            }
        }
        if (!isMatch)
        {
            BackToDefault();
        }
    }

    /// <summary>
    /// Back to deafult position
    /// </summary>
    private void BackToDefault()
    {
        transform.DOLocalMove(defPos, 0.5f).OnComplete(() => { IsCollider(true); });
    }

    /// <summary>
    /// Get material to send color
    /// </summary>
    /// <returns></returns>
    private Color GetColorFromMaterial()
    {
        string colorProperty = null;

        if (meshRenderer.material.HasProperty("_Tint"))
            colorProperty = "_Tint";
        else if (meshRenderer.material.HasProperty("_TintColor"))
            colorProperty = "_TintColor";
        else if (meshRenderer.material.HasProperty("_Color"))
            colorProperty = "_Color";
        else if (meshRenderer.material.HasProperty("_BaseColor"))
            colorProperty = "_BaseColor";

        Color startColor = meshRenderer.material.GetColor(colorProperty);
        return startColor;
    }

    private IEnumerator MatchToFlask(Vector3 targetPos)
    {
        Vector3 currentRotation = transform.localEulerAngles;
        transform.DOMove(targetPos, 0.5f);
        yield return new WaitForSeconds(0.5f);
        // First rotate X to 30 while keeping Y and Z the same
        transform.DOLocalRotate(new Vector3(-90f, currentRotation.y, currentRotation.z), 0.5f)
            .OnComplete(() =>
            {
                StartCoroutine(LerpColorCoroutine(Color.grey, 1.5f));
                transform.DOLocalRotate(new Vector3(0f, currentRotation.y, currentRotation.z), 0.5f).SetDelay(1.5f);
            });

        yield return new WaitForSeconds(2f);
        BackToDefault();
    }


    /// <summary>
    /// Fill Color slowly
    /// </summary>
    /// <param name="targetColor"></param>
    /// <param name="duration"></param>
    /// <returns></returns>
    private IEnumerator LerpColorCoroutine(Color targetColor, float duration)
    {
        Material[] materials = meshRenderer.materials;
        float time = 0f;

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
    }

    /// <summary>
    /// When back to default then slowly fill color to show another solution
    /// </summary>
    private void AutoColorFill()
    {
        // It is use to show test-tube fill another solutions
        int random = Random.Range(0, sampleColors.Length);
        StartCoroutine(LerpColorCoroutine(sampleColors[random], 1.5f));
    }
}
