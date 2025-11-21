using UnityEngine;
namespace Frimus
{
    namespace ECSDestructionToolkit
    {
        [RequireComponent(typeof(Camera))]
        public class SceneCameraController : MonoBehaviour
        {
            public float moveSpeed = 10f;
            public float lookSpeed = 2f;
            public float shiftMultiplier = 3f;

            float yaw;
            float pitch;

            void Start()
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;

                Vector3 euler = transform.eulerAngles;
                yaw = euler.y;
                pitch = euler.x;
            }

            void Update()
            {
                if (!Input.GetMouseButton(1)) return;
                // Look around with mouse
                float mouseX = Input.GetAxis("Mouse X") * lookSpeed;
                float mouseY = Input.GetAxis("Mouse Y") * lookSpeed;

                yaw += mouseX;
                pitch -= mouseY;
                pitch = Mathf.Clamp(pitch, -89f, 89f);

                transform.rotation = Quaternion.Euler(pitch, yaw, 0f);

                // Move with WASD + QE
                float speed = moveSpeed * (Input.GetKey(KeyCode.LeftShift) ? shiftMultiplier : 1f);
                Vector3 direction =
                    new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));

                if (Input.GetKey(KeyCode.E)) direction.y += 1f;
                if (Input.GetKey(KeyCode.Q)) direction.y -= 1f;

                transform.Translate(direction * speed * Time.deltaTime, Space.Self);

                // Escape to unlock mouse
                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    Cursor.lockState = CursorLockMode.None;
                    Cursor.visible = true;
                }
            }
        }
    }
}