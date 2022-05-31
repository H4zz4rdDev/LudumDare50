using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using Color = UnityEngine.Color;

namespace Game
{
    public class PlayerController : MonoBehaviour
    {
        [Header("Manager Settings")] public GameManager gameManager;

        [Header("Animation Settings")] [SerializeField]
        private Animator animator;

        private float _inputX;
        private float _inputY;
        private float _scaleX;
        private float _scaleY;
        private bool _canClimb;
        private bool _isOnLadder;
        private bool _isOnSpill;
        private ParticleSystem _currentSpillParticleSystem;

        [Header("Audio Settings")] public AudioSource spillAudio;
        public AudioSource waterAudio;
        public AudioSource ploppAudio;
        public AudioSource grassAudio;
        public AudioSource metalAudio;
        public AudioSource chopAudio;
        public AudioSource collectSound;


        [Header("Ladder Collision Settings")] [SerializeField]
        private GameObject ladderLeftLeft;

        [SerializeField] private GameObject ladderLeftRight;
        [SerializeField] private GameObject ladderRightLeft;
        [SerializeField] private GameObject ladderRightRight;

        public float movementSpeed;
        public float characterScale;
        public float maxHeight;
        public bool isGrounded;
        public LayerMask groundLayer;
        public LayerMask ladderLayer;
        public float collisionIsGroundedRange = 0.25f;
        public bool isOnChest;
        public bool isOnFarmland;

        [Header("GUI Settings")] public GameObject headImage;

        public TMP_Text flowerUiScore;
        public TMP_Text weedUiScore;
        public TMP_Text finalScore;
        public TMP_Text levelUi;
        public TMP_Text nextLevelUi;
        public int WeedCoins;
        public int FlowerCoins;


        public Tilemap tilemap;
        public FlowerBedManager flowerBedManager;
        public bool isWaterIconShown;
        public int waterIconShowTime;
        private float _elapsedWaterIconTime;
        
        void Update()
        {
            if (gameManager.gamestate == Gamestate.Start)
            {
            }
            else if (gameManager.gamestate == Gamestate.Play)
            {
                if (isWaterIconShown)
                {
                    if (_elapsedWaterIconTime >= waterIconShowTime)
                    {
                        headImage.SetActive(false);
                        _elapsedWaterIconTime = 0;
                        isWaterIconShown = false;
                    }

                    _elapsedWaterIconTime += Time.deltaTime;
                }

                if (flowerBedManager.IsWaterRunning())
                {
                    if (!waterAudio.isPlaying)
                    {
                        waterAudio.Play();
                    }
                }
                else
                {
                    waterAudio.Stop();
                }

                isGrounded = Physics2D.OverlapCircle(transform.position, collisionIsGroundedRange, groundLayer);
                _isOnLadder = Physics2D.OverlapPoint(new Vector2(transform.position.x, transform.position.y - 0.1f),
                    ladderLayer);

                if (!isGrounded)
                {
                    if (animator.GetBool("Harvest"))
                    {
                        animator.SetBool("Harvest", false);
                    }

                    if (ladderRightLeft.activeSelf != true)
                    {
                        ladderRightLeft.SetActive(true);
                        ladderRightRight.SetActive(true);
                    }

                    if (ladderLeftLeft.activeSelf != true)
                    {
                        ladderLeftLeft.SetActive(true);
                        ladderLeftRight.SetActive(true);
                    }
                }
                else
                {
                    if (_inputX != 0 && isGrounded)
                    {
                        if (!grassAudio.isPlaying)
                        {
                            grassAudio.Play();
                        }
                    }

                    if (_inputX != 0 && !isGrounded)
                    {
                        if (!metalAudio.isPlaying)
                        {
                            metalAudio.Play();
                        }
                    }

                    if (_inputY > 0)
                    {
                        if (isOnChest && Keyboard.current.spaceKey.wasPressedThisFrame)
                        {
                            var oldFinalScore = Int32.Parse(finalScore.text);
                            var weedCount = Int32.Parse(weedUiScore.text);
                            var flowerCount = Int32.Parse(flowerUiScore.text);

                            var newFinalScore = oldFinalScore + (weedCount * WeedCoins) + (flowerCount * FlowerCoins);

                            if (newFinalScore >= gameManager.currentLevel.ScoreNeeded)
                            {
                                gameManager.currentLevel =
                                    gameManager.levelConfigs[gameManager.currentLevel.LevelId + 1];
                                finalScore.text = finalScore.text = "000000";

                                levelUi.text = gameManager.currentLevel.LevelId.ToString();
                                nextLevelUi.text = gameManager.currentLevel.ScoreNeeded.ToString();
                            }
                            else
                            {

                                if (newFinalScore < 10)
                                {
                                    finalScore.text = "00000" + newFinalScore;
                                }
                                else if (newFinalScore < 100)
                                {
                                    finalScore.text = "0000" + newFinalScore;
                                }
                                else if (newFinalScore < 1000)
                                {
                                    finalScore.text = "000" + newFinalScore;
                                }
                                else if (newFinalScore < 10000)
                                {
                                    finalScore.text = "00" + newFinalScore;
                                }
                                else if (newFinalScore < 100000)
                                {
                                    finalScore.text = "0" + newFinalScore;
                                }
                                else
                                {
                                    finalScore.text = newFinalScore.ToString();
                                }
                            }

                            weedUiScore.text = "000";
                            flowerUiScore.text = "000";
                            
                            collectSound.Play();
                        }
                        else if (isOnFarmland && Keyboard.current.spaceKey.wasPressedThisFrame)
                        {
                            //Harvest
                            if (Keyboard.current.spaceKey.wasPressedThisFrame)
                            {
                                var tileMapPositionX = tilemap.WorldToCell(transform.position).x;

                                if (flowerBedManager.HasWaterOnFlowerBed(tileMapPositionX))
                                {
                                    if (!isWaterIconShown)
                                    {
                                        headImage.SetActive(true);
                                        isWaterIconShown = true;
                                    }
                                }

                                if (flowerBedManager.CanHarvest(tileMapPositionX))
                                {
                                    flowerBedManager.Harvest(tileMapPositionX);
                                }
                            }
                        }

                        if (!animator.GetBool("Harvest"))
                        {
                            animator.SetBool("Harvest", true);
                        }
                    }
                    else
                    {
                        if (animator.GetBool("Harvest"))
                        {
                            animator.SetBool("Harvest", false);
                        }
                    }

                    ladderLeftLeft.SetActive(false);
                    ladderLeftRight.SetActive(false);
                    ladderRightLeft.SetActive(false);
                    ladderRightRight.SetActive(false);
                }

                if (_isOnSpill)
                {
                    if (Keyboard.current.spaceKey.wasPressedThisFrame)
                    {
                        spillAudio.Play();

                        var tileMapPositionX = tilemap.WorldToCell(transform.position).x;

                        if (_currentSpillParticleSystem.isEmitting)
                        {
                            _currentSpillParticleSystem.Stop();
                            flowerBedManager.SetWaterForSpill(tileMapPositionX);
                        }
                        else
                        {
                            _currentSpillParticleSystem.Play();
                            flowerBedManager.SetWaterForSpill(tileMapPositionX, true);
                        }
                    }
                }


                if (!animator.GetBool("Harvest"))
                {
                    transform.position += new Vector3((_inputX * movementSpeed) * Time.deltaTime, 0, 0);
                    _scaleX = _inputX > 0f ? -characterScale : characterScale;
                    animator.SetFloat("Speed", Mathf.Abs(_inputX));

                    transform.localScale = new Vector3(_scaleX, characterScale, characterScale);
                }

                if (_inputY != 0f && (_canClimb || _isOnLadder))
                {
                    if (transform.position.y > maxHeight)
                    {
                        var transform1 = transform;
                        transform1.position = new Vector3(transform1.position.x, maxHeight, 0);
                    }

                    transform.position += new Vector3(0, (_inputY * movementSpeed) * Time.deltaTime, 0);
                    _scaleY = _inputY > 0f ? -characterScale : characterScale;
                    animator.SetFloat("Speed", Mathf.Abs(_inputY));
                    animator.SetBool("Climb", true);
                }
                else
                {
                    animator.SetBool("Climb", false);
                }
            }
            else if (gameManager.gamestate == Gamestate.GameOver)
            {
            }
        }

        public void PlayPlopp()
        {
            ploppAudio.Play();
        }

        public void PlayChop()
        {
            chopAudio.Play();
        }

        public void Move(InputAction.CallbackContext context)
        {
            _inputX = context.ReadValue<Vector2>().x;
            _inputY = context.ReadValue<Vector2>().y;

            //print(_inputX + " ----- " + _inputY);
        }

        private void OnTriggerEnter2D(Collider2D col)
        {
            if (col.gameObject.layer == LayerMask.NameToLayer("Spill"))
            {
                _isOnSpill = true;
                _currentSpillParticleSystem = col.gameObject.GetComponent<ParticleSystem>();
            }

            if (col.name == "Ladder")
            {
                _canClimb = true;
            }

            if (col.gameObject.layer == LayerMask.NameToLayer("Chest"))
            {
                isOnChest = true;
            }

            if (col.gameObject.layer == LayerMask.NameToLayer("Farmland"))
            {
                isOnFarmland = true;
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.gameObject.layer == LayerMask.NameToLayer("Spill"))
            {
                _isOnSpill = false;
            }

            if (other.name == "Ladder")
            {
                _canClimb = false;
            }

            if (other.gameObject.layer == LayerMask.NameToLayer("Chest"))
            {
                isOnChest = false;
            }

            if (other.gameObject.layer == LayerMask.NameToLayer("Farmland"))
            {
                isOnFarmland = false;
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, collisionIsGroundedRange);
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(transform.position, new Vector3(transform.position.x, transform.position.y - 0.1f, 0));
        }
    }
}