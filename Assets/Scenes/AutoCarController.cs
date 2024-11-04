using UnityEngine;
using System.Collections.Generic;
using TMPro;

namespace PolyStang
{
    public class AutoDriveController : MonoBehaviour
    {
        [Header("Target")]
        public Transform target; // El objetivo que el coche debe seguir.

        [Header("Car Settings")]
        public float maxAcceleration = 30.0f;
        public float maxSteerAngle = 30.0f;
        public float targetReachedThreshold = 2.0f; // Distancia mínima al objetivo para detenerse.
        public float stoppingDistance = 5.0f; // Distancia mínima para reducir la velocidad.

        private Rigidbody carRb;
        private List<CarController.Wheel> wheels;
        private CarController carController;

        private float moveInput = 0f;
        private float steerInput = 0f;

        void Start()
        {
            carController = GetComponent<CarController>();
            carRb = GetComponent<Rigidbody>();
            wheels = carController.wheels;
        }

        void Update()
        {
            if (target != null)
            {
                // Calcula los inputs necesarios para alcanzar el objetivo
                CalculateInputs();
                ApplyInputs();
            }
        }

        void CalculateInputs()
        {
            // Calcula la dirección hacia el objetivo
            Vector3 directionToTarget = (target.position - transform.position).normalized;
            float distanceToTarget = Vector3.Distance(transform.position, target.position);

            // Calcula el ángulo de dirección necesario
            Vector3 localTarget = transform.InverseTransformPoint(target.position);
            steerInput = Mathf.Clamp(localTarget.x / localTarget.magnitude, -1, 1) * carController.turnSensitivity;

            // Acelerar o frenar en función de la distancia al objetivo
            if (distanceToTarget > stoppingDistance)
            {
                moveInput = 1f; // Acelerar
            }
            else if (distanceToTarget > targetReachedThreshold)
            {
                moveInput = Mathf.Lerp(1f, 0f, (stoppingDistance - distanceToTarget) / stoppingDistance); // Reducir velocidad
            }
            else
            {
                moveInput = 0f; // Detenerse
            }
        }

        void ApplyInputs()
        {
            foreach (var wheel in wheels)
            {
                // Aceleración
                float motorTorque = moveInput * 600 * maxAcceleration * Time.deltaTime;
                wheel.wheelCollider.motorTorque = motorTorque;

                // Dirección (solo en ruedas delanteras)
                if (wheel.axel == CarController.Axel.Front)
                {
                    float steerAngle = steerInput * maxSteerAngle;
                    wheel.wheelCollider.steerAngle = Mathf.Lerp(wheel.wheelCollider.steerAngle, steerAngle, 0.6f);
                }
            }
        }

        void OnDrawGizmos()
        {
            // Visualización de la línea hacia el objetivo en el editor.
            if (target != null)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawLine(transform.position, target.position);
            }
        }
    }
}
