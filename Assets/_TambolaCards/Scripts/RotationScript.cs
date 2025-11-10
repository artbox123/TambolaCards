using UnityEngine;

namespace ArtboxGames
{
    public class RotationScript : MonoBehaviour
    {
        public float speed = 30f;
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            transform.Rotate(new Vector3(0, 0, Time.deltaTime * speed));
        }
    }
}