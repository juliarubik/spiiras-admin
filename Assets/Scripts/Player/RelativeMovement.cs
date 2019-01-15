using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RelativeMovement : MonoBehaviour
{
    //Окружающие строки показывают контекст размещения метода RequireComponent().
    [SerializeField] private Transform target; //Сценарию нужна ссылка на объект, относительно которого будет происходить перемещение.

    public float jumpSpeed = 15.0f;
    public float gravity = -9.8f;
    public float terminalVelocity = -10.0f;
    public float minFall = -1.5f;

    private float _vertSpeed;
    private ControllerColliderHit _contact;

    public float moveSpeed = 6.0f;
    public float rotSpeed = 15.0f; //Чтобы движение было плавным.

    private CharacterController _charController;

    void Start()
    {
        _charController = GetComponent<CharacterController>(); //Этот паттерн используется для доступа к другим компонентам.
        _vertSpeed = minFall; //Инициализируем скорость по вертикали, присваивая ей минимальную скорость падения в начале существующей функции.
    }

    void Update()
    {
        Vector3 movement = Vector3.zero; //Начинаем с вектора(0, 0, 0), непрерывно добавляя компоненты движения.

        float horInput = Input.GetAxis("Horizontal");
        float vertInput = Input.GetAxis("Vertical");
        if (horInput != 0 || vertInput != 0)
        {
            //Движение обрабатывается только при нажатии клавиш со стрелками.

            movement.x = horInput * moveSpeed; //Добавляем скорость движения
            movement.z = vertInput * moveSpeed;
            movement = Vector3.ClampMagnitude(movement, moveSpeed); //Ограничиваем движение по диагонали той же скоростью, что и движение вдоль оси.
            Quaternion tmp = target.rotation; //Сохраняем начальную ориентацию, чтобы вернуться к ней после завершения работы с целевым объектом.

            target.eulerAngles = new Vector3(0, target.eulerAngles.y, 0);
            movement = target.TransformDirection(movement); //Преобразуем направление движения из локальных в глобальные координаты.
            target.rotation = tmp;
            Quaternion direction = Quaternion.LookRotation(movement); //Метод LookRotation() вычисляет кватернион, смотрящий в этом направлении.
            transform.rotation = Quaternion.Lerp(transform.rotation,
            direction, rotSpeed * Time.deltaTime);
        }

        bool hitGround = false;
        RaycastHit hit;
        if (_vertSpeed < 0 && //Проверяем, падает ли персонаж.
        Physics.Raycast(transform.position, Vector3.down, out hit))
        {
            float check = (_charController.height + _charController.radius) / 1.9f; //Расстояние, с которым производится сравнение (слегка выходит за нижнюю часть капсулы).
            hitGround = hit.distance <= check;
        }

        if (hitGround)  //смотрим на результат бросания луча
        { 
            if (Input.GetButtonDown("Jump")) // Реакция на кнопку Jump при нахождении на поверхности.
            {
                //Реакция на кнопку Jump при нахождении на поверхности.
                _vertSpeed = jumpSpeed;
            }
            else
            {
                _vertSpeed = minFall;
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

            if (_charController.isGrounded) //Метод бросания луча не обнаруживает поверхности, но капсула с ней соприкасается.
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

                movement.y = _vertSpeed;
            movement *= Time.deltaTime; //Не забываем умножать перемещения на значение deltaTime, чтобы они не зависели от частоты кадров.
            _charController.Move(movement);
        }
  
    }
}
