using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Knife : MonoBehaviour
{
    [SerializeField] GameObject finishPanel;
    [SerializeField] Image levelImage;
    [SerializeField] Sprite[] levelImages;

    [SerializeField] Level[] levels;
    public int currentLevel;
    public bool levelCompleted;

    [SerializeField] float damage;

    [SerializeField] Wood wood;
    [SerializeField] ParticleSystem particle;
    [SerializeField] ParticleSystem.EmissionModule particleEmission;

    [SerializeField] float speed;
    [SerializeField] float minX;
    [SerializeField] float maxX;
    [SerializeField] float minY;
    [SerializeField] float maxY;

    bool isMoving = false;

    Rigidbody rb;
    Vector3 desiredPos;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        particleEmission = particle.emission;
        currentLevel = PlayerPrefs.GetInt("Level");
        if(currentLevel > levels.Length)
        {
            Application.Quit();
        }
        levelImage.sprite = levelImages[currentLevel];
    }

    // Update is called once per frame
    void Update()
    {
        if (!levelCompleted)
        {
            isMoving = Input.GetMouseButton(0);
            if (isMoving)
            {
                desiredPos = new Vector3(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"), 0) * speed * Time.fixedDeltaTime;
            }
        }
        else
        {
            desiredPos = Vector3.zero;
        }
    }
    private void FixedUpdate()
    {
        if (isMoving)
        {
            rb.position += desiredPos;
            rb.position = new Vector3(Mathf.Clamp(rb.position.x,minX,maxX),Mathf.Clamp(rb.position.y,minY,maxY));
        }
    }
    private void OnCollisionStay(UnityEngine.Collision collision)
    {
        Coll coll = collision.gameObject.GetComponent<Coll>();
        if(coll != null)
        {
            coll.HitCollider(damage);
            wood.HitMesh(coll.index,damage);

            particle.transform.position = collision.contacts[0].point;
            particleEmission.enabled = true;

            CheckLevel(currentLevel);
        }
    }
    private void OnCollisionExit(Collision collision)
    {
        particleEmission.enabled = false;
    }
    void CheckLevel(int levelIndex)
    {
        List<int> passedIndexes = new List<int>();
        for (int i = 0; i < levels[levelIndex].data.Length; i++)
        {
            if(wood.skinnedMeshRenderer.GetBlendShapeWeight(i) - 3 >= levels[levelIndex].data[i] + 3 || wood.skinnedMeshRenderer.GetBlendShapeWeight(i) + 3 > levels[levelIndex].data[i] - 3)
            {
                if (!passedIndexes.Contains(i))
                {
                    passedIndexes.Add(i);
                }
            }
            else
            {
                if (passedIndexes.Contains(i))
                {
                    passedIndexes.Remove(i);
                }
            }
        }
        if(passedIndexes.Count == wood.skinnedMeshRenderer.sharedMesh.blendShapeCount)
        {
            if (!levelCompleted)
            {
                levelCompleted = true;
                Debug.Log("Level Completed !");
                PlayerPrefs.SetInt("Level",currentLevel+1);
                finishPanel.SetActive(true);
            }
        }
    }
    public void NextButton()
    {
        SceneManager.LoadScene(0);
    }
    public void ExitButton()
    {
        Application.Quit();
    }
}
