using System.Collections;
using UnityEngine;

public class MouseLookStable : MonoBehaviour
{
    [Header("Configurações")]
    public float mouseSensitivity = 100f;
    public Transform playerBody;
    public bool podeMover = true;

    private float xRotation = 0f; // Pitch acumulado

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        SincronizarComRotacaoAtual();
        Input.ResetInputAxes();
    }

    void Update()
    {
        if (!podeMover) return;

        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        SincronizarComRotacaoAtual();

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        // Aplica pitch acumulado mantendo yaw atual
        transform.localEulerAngles = new Vector3(xRotation, transform.localEulerAngles.y, 0f);

        // Aplica yaw no corpo do player
        playerBody.Rotate(Vector3.up * mouseX);
    }

    public void TravarMovimento(float segundos)
    {
        StartCoroutine(TravarPorTempo(segundos));
    }

    private IEnumerator TravarPorTempo(float segundos)
    {
        podeMover = false;
        yield return new WaitForSeconds(segundos);
        Input.ResetInputAxes();
        podeMover = true;
    }

    private float ToSignedAngle(float angle)
    {
        return (angle > 180f) ? angle - 360f : angle;
    }

    private void SincronizarComRotacaoAtual()
    {
        // Ajusta o acumulado para bater com a rotação atual
        xRotation = ToSignedAngle(transform.localEulerAngles.x);
    }
}
