# Introducción

En la última década, la transformación digital ha redefinido la relación entre los ciudadanos
y el Estado. Los servicios públicos digitales —conocidos como *gobierno electrónico* o
*e-government*— han demostrado ser instrumentos eficaces para reducir la ineficiencia
burocrática, incrementar la transparencia y limitar la discrecionalidad que favorece la
corrupción en la atención presencial. La evidencia internacional señala que la digitalización
del sector público tiene un efecto positivo y significativo sobre el control de la corrupción
y la efectividad gubernamental, especialmente en los países en desarrollo, donde la corrupción
y los flujos financieros ilícitos representan pérdidas superiores al billón de dólares anuales.

En el contexto nacional, el Perú ha avanzado en la construcción de plataformas digitales
estatales; sin embargo, persiste una **brecha crítica de digitalización a nivel municipal**.
De acuerdo con reportes recientes, solo el 31 % de las municipalidades cuenta con una Mesa de
Partes Digital y apenas 199 de los 1 891 municipios del país tenían operativo el Sistema Único
de Trámites durante 2024. Aunque más del 93 % de los gobiernos locales migró su portal al
dominio institucional gob.pe, disponer de un sitio web no equivale a contar con procesos
digitales ni interoperables. A esta brecha se suma que las municipalidades generan tres de
cada cuatro barreras burocráticas identificadas por el Indecopi y encabezan los casos de
corrupción en trámite registrados por la Defensoría del Pueblo.

Aterrizando el análisis en la región de Ayacucho, la situación reproduce el patrón nacional:
la Municipalidad Provincial de Huamanga figuró como la entidad más quejada ante la Defensoría
del Pueblo durante 2021 y 2022, y la atención a la ciudadanía continúa siendo predominantemente
presencial y en horario de oficina. Tomando como caso de referencia la **Municipalidad
Distrital de San Juan Bautista**, se observa que el ciudadano debe acudir físicamente para
iniciar y dar seguimiento a sus trámites, lo que se traduce en pérdida de tiempo, sobrecostos,
opacidad en el estado del expediente y exposición a prácticas irregulares.

A esta problemática operativa se añade una dimensión legal ineludible: el tratamiento de datos
personales de la ciudadanía. Con la entrada en vigencia del nuevo Reglamento de la **Ley
N.° 29733 – Ley de Protección de Datos Personales** (Decreto Supremo N.° 016-2024-JUS, vigente
desde marzo de 2024), las entidades públicas están obligadas a implementar medidas técnicas de
seguridad, designar un Oficial de Datos Personales, atender los derechos de los titulares y
notificar los incidentes de seguridad, bajo un régimen sancionador que en 2024 impuso multas
por más de 13 millones de soles. La mayoría de municipios no está preparado para cumplir estas
obligaciones.

Frente a este escenario surge la pregunta de investigación principal: **¿cuáles son los
resultados del desarrollo de una plataforma web para la gestión de trámites municipales con
enfoque en seguridad y protección de datos personales, 2026?** Para responderla, el objetivo
general del proyecto es desarrollar dicha plataforma, digitalizando el ciclo completo del
trámite —inicio, pago, seguimiento y resolución— y garantizando la trazabilidad y la
confidencialidad de la información mediante controles de seguridad desde el diseño.

El proyecto se justifica en tres planos. **Prácticamente**, provee una herramienta que reduce
la fricción y la opacidad del trámite presencial, acercando el servicio municipal al ciudadano.
**Socialmente**, promueve la transparencia y la equidad en el acceso a los servicios públicos,
al eliminar la discrecionalidad de la ventanilla y registrar cada actuación de forma auditable.
**Metodológicamente**, demuestra la viabilidad de aplicar **Spec-Driven Development (SDD)**
—complementado con Scrum y Extreme Programming— y una arquitectura moderna (Clean Architecture
sobre .NET 10) para construir software público de calidad, seguro y conforme a la normativa de
protección de datos.

En cuanto a sus alcances y delimitaciones, el proyecto se enfoca en el desarrollo de un
Producto Mínimo Viable (MVP). La validación de las variables de funcionamiento y usabilidad se
delimita a un entorno de pruebas controlado y a una muestra piloto no probabilística, adecuada
para esta fase inicial. La integración con pasarelas de pago y con sistemas externos
(RENIEC, SUNAT) se contempla en modo de prueba, quedando su implementación productiva como
línea de trabajo futura.

Finalmente, el informe se organiza en cinco capítulos. El **Capítulo I** presenta el
planteamiento del problema, los objetivos y la justificación. El **Capítulo II** desarrolla el
marco teórico, abarcando los antecedentes y las bases conceptuales del gobierno digital, la
ingeniería de software y la protección de datos. El **Capítulo III** describe el material y los
métodos, el tipo de investigación y la integración de las metodologías (SDD, Scrum y XP). El
**Capítulo IV** expone los resultados del desarrollo por fases y la discusión sobre la
validación funcional, el cumplimiento de seguridad y la evaluación de la usabilidad. Por último,
el **Capítulo V** presenta las conclusiones y las recomendaciones para futuras líneas de
trabajo.
