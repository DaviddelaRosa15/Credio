# Credio 🏦
**Sistema Integral de Gestión de Créditos y Cobranza (SGCC)**

![Estado del Proyecto](https://img.shields.io/badge/Estado-En%20Desarrollo-orange?style=flat-square)
![Licencia](https://img.shields.io/badge/Licencia-MIT-blue?style=flat-square)
![Versión](https://img.shields.io/badge/Versión-MVP%201.0-green?style=flat-square)

## 📖 Descripción del Proyecto

**Credio** es una solución tecnológica diseñada para optimizar el ciclo de vida de los préstamos en instituciones de microfinanzas. El sistema centraliza la operación desde la originación del crédito hasta la recuperación de cartera en campo.

A diferencia de los sistemas tradicionales, Credio se enfoca en la **movilidad y la integridad financiera**, ofreciendo herramientas de geolocalización para oficiales de cobro y un motor de cálculo bancario robusto, todo accesible desde una Web App responsiva sin necesidad de hardware costoso.

## 🚀 Funcionalidades Principales

### 🧮 Motor Financiero (Core)
* Soporte para múltiples sistemas de amortización: **Francés, Alemán y Americano**.
* Cálculo automático de mora y gestión de pagos parciales/adelantados.
* Precisión decimal para garantizar integridad contable.

### 📍 Logística y Cobranza (Field Ops)
* **Web App Móvil** para cobradores con asignación de rutas diarias.
* **Geolocalización (Anti-Fraude):** Registro de coordenadas GPS al momento del cobro para auditar visitas.
* **Comprobantes Digitales:** Generación de recibos en PDF y envío directo vía WhatsApp.

### 📂 Gestión Documental & CRM
* Expediente digital del cliente (KYC).
* Generación automática de documentos legales (**Pagaré Notarial**, Carta de Saldo).

### 📊 Inteligencia de Negocio
* Dashboard de Liquidez (Dinero en Banco vs. Dinero en Calle).
* Reportes de antigüedad de saldos y eficiencia de cobranza.

---

## 🛠️ Stack Tecnológico

La arquitectura de **Credio** está diseñada para ser escalable, segura y modular.

* **Base de Datos:** PostgreSQL 🐘 (Integridad referencial y transacciones ACID).
* **Backend:** [C# .NET 8 / Python] (API RESTful).
* **Frontend Web:** [React / Vue.js ] + TailwindCSS.
* **Infraestructura:** Docker containers.
* **Servicios Externos:** API de Geolocalización HTML5.
