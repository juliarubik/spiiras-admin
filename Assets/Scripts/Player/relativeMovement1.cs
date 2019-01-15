using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CharacterController))]
public class relativeMovement1 : MonoBehaviour
{
    [SerializeField] private Transform target; //Сценарию нужна ссылка на объект, относительно которого будет происходить перемещение.

    public float moveSpeed = 6.0f;
    public float rotSpeed = 15.0f;   //Чтобы движение было плавным.
    public float jumpSpeed = 15.0f;
    public float gravity = -9.8f;
    public float terminalVelocity = -20.0f;
    public float minFall = -1.5f;

    private float _vertSpeed;
    private ControllerColliderHit _contact;

    private CharacterController _charController;
    private Animator _animator;

    // Use this for initialization
    void Start()
    {
        _vertSpeed = minFall;  //Инициализируем скорость по вертикали, присваивая ей минимальную скорость падения в начале существующей функции.

        _charController = GetComponent<CharacterController>();  //Этот паттерн используется для доступа к другим компонентам.
        _animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {

        //Начинаем с вектора(0, 0, 0), непрерывно добавляя компоненты движения.
        Vector3 movement = Vector3.zero;

        float horInput = Input.GetAxis("Horizontal");
        float vertInput = Input.GetAxis("Vertical");
        if (horInput != 0 || vertInput != 0)
        {
            //Движение обрабатывается только при нажатии клавиш со стрелками.
            movement.x = horInput * moveSpeed;
            movement.z = vertInput * moveSpeed;
            movement = Vector3.ClampMagnitude(movement, moveSpeed);

            Quaternion tmp = target.rotation;
            target.eulerAngles = new Vector3(0, target.eulerAngles.y, 0);
            movement = target.TransformDirection(movement);
            target.rotation = tmp;

            // face movement direction
            //transform.rotation = Quaternion.LookRotation(movement);
            Quaternion direction = Quaternion.LookRotation(movement);
            transform.rotation = Quaternion.Lerp(transform.rotation,
                                                 direction, rotSpeed * Time.deltaTime);
        }
        _animator.SetFloat("Speed", movement.sqrMagnitude);

        bool hitGround = false;
        RaycastHit hit;
        if (_vertSpeed < 0 && Physics.Raycast(transform.position, Vector3.down, out hit))  //Проверяем, падает ли персонаж.
        {
            float check = (_charController.height + _charController.radius) / 1.9f;  //Расстояние, с которым производится сравнение (слегка выходит за нижнюю часть капсулы).
            hitGround = hit.distance <= check;  
        }

        // y movement: possibly jump impulse up, always accel down
        // could _charController.isGrounded instead, but then cannot workaround dropoff edge
        if (hitGround)
        {
            if (Input.GetButtonDown("Jump"))  // Реакция на кнопку Jump при нахождении на поверхности.
            {
                _vertSpeed = jumpSpeed;
            }
            else
            {
                _vertSpeed = minFall;
                _animator.SetBool("Jumping", false);
            }
        }
        else
        {
            //Если персонаж не стоит на поверхности, применяем гравитацию, пока не будет достигнута предельная скорость.

            _vertSpeed += gravity * 5 * Time.deltaTime;
            if (_vertSpeed < terminalVelocity)
            {
                _vertSpeed = terminalVelocity;
            }
            if (_contact != null)
            {   // not right at level start
                _animator.SetBool("Jumping", true);
            }

            //Метод бросания луча не обнаруживает поверхности, но капсула с ней соприкасается.
            if (_charController.isGrounded)
            {
                if (Vector3.Dot(movement, _contact.normal) < 0)  //Реакция слегка меняется в зависимости от того, смотрит ли персонаж в сторону точки контакта.
                {
                    movement = _contact.normal * moveSpeed;
                }
                else
                {
                    movement += _contact.normal * moveSpeed;
                }
            }
        }
        movement.y = _vertSpeed;

        movement *= Time.deltaTime;   //Умножаем перемещения на значение deltaTime, чтобы они не зависели от частоты кадров.
        _charController.Move(movement);
    }

    // store collision to use in Update
    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        _contact = hit;
    }
}