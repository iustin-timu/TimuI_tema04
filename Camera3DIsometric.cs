using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace ConsoleApp3
{
    class Camera3DIsometric
    {
        private Vector3 eye;
        private Vector3 target;
        private Vector3 up_vector;

        // Viteză de mișcare
        private const float MOVEMENT_UNIT = 0.5f;

        public Camera3DIsometric()
        {
            eye = new Vector3(30, 15, 30);
            target = new Vector3(0, 0, 0);
            up_vector = new Vector3(0, 1, 0);
        }

        public Camera3DIsometric(int _eyeX, int _eyeY, int _eyeZ, int _targetX, int _targetY, int _targetZ, int _upX, int _upY, int _upZ)
        {
            eye = new Vector3(_eyeX, _eyeY, _eyeZ);
            target = new Vector3(_targetX, _targetY, _targetZ);
            up_vector = new Vector3(_upX, _upY, _upZ);
        }

        public Camera3DIsometric(Vector3 _eye, Vector3 _target, Vector3 _up)
        {
            eye = _eye;
            target = _target;
            up_vector = _up;
        }

        public void SetCamera()
        {
            Matrix4 camera = Matrix4.LookAt(eye, target, up_vector);
            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadMatrix(ref camera);
        }

        // --- METODE DE MIȘCARE 3D (CU LOGICA CORECTATĂ/INVERSATĂ) ---

        public void MoveForward()
        {
            // W (Înainte)
            Vector3 direction = target - eye;
            direction.Normalize();
            eye += direction * MOVEMENT_UNIT;
            target += direction * MOVEMENT_UNIT;
            SetCamera();
        }

        public void MoveBackward()
        {
            // S (Înapoi)
            Vector3 direction = target - eye;
            direction.Normalize();
            eye -= direction * MOVEMENT_UNIT;
            target -= direction * MOVEMENT_UNIT;
            SetCamera();
        }

        public void MoveLeft()
        {
            // A (Stânga)
            Vector3 direction = target - eye;
            Vector3 right = Vector3.Cross(direction, up_vector);
            right.Normalize();

            eye -= right * MOVEMENT_UNIT;
            target -= right * MOVEMENT_UNIT;
            SetCamera();
        }

        public void MoveRight()
        {
            // D (Dreapta)
            Vector3 direction = target - eye;
            Vector3 right = Vector3.Cross(direction, up_vector);
            right.Normalize();

            eye += right * MOVEMENT_UNIT;
            target += right * MOVEMENT_UNIT;
            SetCamera();
        }

        public void MoveUp()
        {
            // Q (Sus)
            Vector3 direction = target - eye;
            Vector3 right = Vector3.Cross(direction, up_vector);
            Vector3 up = Vector3.Cross(right, direction);
            up.Normalize();

            eye += up * MOVEMENT_UNIT;
            target += up * MOVEMENT_UNIT;
            SetCamera();
        }

        public void MoveDown()
        {
            // E (Jos)
            Vector3 direction = target - eye;
            Vector3 right = Vector3.Cross(direction, up_vector);
            Vector3 up = Vector3.Cross(right, direction);
            up.Normalize();

            eye -= up * MOVEMENT_UNIT;
            target -= up * MOVEMENT_UNIT;
            SetCamera();
        }

        public void Reset(Vector3 newEye, Vector3 newTarget, Vector3 newUp)
        {
            eye = newEye;
            target = newTarget;
            up_vector = newUp;
            SetCamera();
        }
    }
}