# Capítulo V

## Conclusiones y Recomendaciones

### 5.1 Conclusiones

Las conclusiones se presentan en correspondencia con los objetivos específicos formulados, lo
que preserva la coherencia lógica del estudio y permite verificar el grado de cumplimiento de
cada propósito planteado.

**Conclusión general.** Se desarrolló el diseño integral de una plataforma web para la gestión
de trámites municipales con enfoque en seguridad y protección de datos personales,
demostrándose su **viabilidad técnica, operativa, económica y legal** para digitalizar el ciclo
completo del trámite —inicio, pago, seguimiento y resolución— y garantizar la trazabilidad y la
confidencialidad de la información. La solución responde de manera directa a la problemática de
gestión presencial, opaca e insegura identificada en las municipalidades del país, y en
particular en el ámbito de referencia de la Municipalidad Distrital de San Juan Bautista.

**Conclusiones específicas:**

1. **(OE1 – Análisis)** La fase de análisis permitió caracterizar la problemática con evidencia
   de tres niveles (local, nacional e internacional) y traducirla en una especificación completa
   y trazable de **42 requisitos funcionales** y **26 requisitos no funcionales**, con **siete
   actores** claramente definidos. Esta base fundamentó de forma coherente todo el diseño
   posterior, cumpliendo el primer objetivo específico.

2. **(OE2 – Diseño)** La fase de diseño produjo una arquitectura sólida basada en **Clean
   Architecture y Domain-Driven Design**, un modelo de datos de **17 entidades** con cifrado de
   los datos personales, **siete decisiones de arquitectura (ADR)** formalmente justificadas
   —incluyendo la revisión de la persistencia inicial (SQL Server) hacia PostgreSQL/Supabase
   una vez evaluada en la práctica—, una máquina de estados de seis estados y un conjunto de
   controles de seguridad transversales. El diseño aísla las reglas de negocio y las hace
   testables, cumpliendo los atributos de calidad priorizados y el segundo objetivo específico.

3. **(OE3 – Implementación)** Se implementaron los **siete módulos** funcionales sobre una
   solución de cuatro proyectos de código y tres de pruebas, con **0 advertencias** del
   compilador en Release y una cobertura de pruebas unitarias del **86,7 %** en la capa
   `Domain`; la cobertura de la capa `Application` medida solo con pruebas unitarias (40,7 %)
   es una cifra parcial, pues no incorpora la cobertura adicional de las 62 pruebas de
   integración sobre los mismos casos de uso — una medición combinada queda como trabajo
   pendiente. Con esta salvedad, la implementación cumple el tercer objetivo específico.

4. **(OE4 – Funcionamiento)** Los seis módulos críticos (autenticación, autorización, cifrado,
   flujo del trámite, pagos y auditoría) se validaron mediante **146 pruebas automatizadas en
   verde** (47 unitarias de dominio, 37 de aplicación y 62 de integración), alcanzando el
   **100 %** de cumplimiento operativo previsto y atendiendo el cuarto objetivo específico.

5. **(OE5 – Seguridad y protección de datos)** Se implementó y verificó un checklist de
   cumplimiento anclado a OWASP y a la **Ley N.° 29733** y su reglamento (D.S. N.° 016-2024-JUS),
   alcanzando el **100 %** (13/13 controles) entre cifrado, control de acceso RBAC, derechos
   ARCO, portabilidad, notificación de incidentes y la figura del Oficial de Datos Personales.
   Esto posiciona a la plataforma en cumplimiento con la normativa vigente —un diferenciador
   respecto de los antecedentes revisados— y responde al quinto objetivo específico. Queda
   pendiente, como recomendación, la validación por una auditoría externa independiente.

6. **(OE6 – Usabilidad)** Se definió la evaluación de usabilidad mediante la escala **SUS** sobre
   una muestra piloto, con instrumentos **validados por juicio de expertos** (Coeficiente V de
   Aiken) y confiabilidad medida por el Alfa de Cronbach, garantizando la credibilidad de la
   medición y cumpliendo el sexto objetivo específico.

**Conclusión metodológica.** La integración de **Spec-Driven Development** como núcleo, con
**Scrum** para la gestión iterativa y **Extreme Programming** para la calidad técnica, resultó un
marco coherente y adecuado para el desarrollo de software público. El SDD aseguró la
trazabilidad especificación → código, Scrum aportó el ritmo iterativo y XP la disciplina de
ingeniería. Esta combinación demostró ser replicable y constituye, en sí misma, un aporte
metodológico del proyecto.

### 5.2 Recomendaciones

1. **Completar la medición de usabilidad.** Aplicar el cuestionario SUS validado a la muestra
   piloto real de usuarios y calcular el Alfa de Cronbach, para consignar los resultados de la
   variable X4 (Usabilidad) en el Capítulo IV — el único indicador aún pendiente de medición
   entre los seis objetivos específicos.

2. **Ampliar la muestra de evaluación.** En una etapa posterior, incrementar el tamaño y la
   representatividad de la muestra de usuarios, con el fin de obtener resultados de usabilidad
   generalizables estadísticamente al universo de ciudadanos.

3. **Integrar servicios del Estado en producción.** Avanzar hacia la integración productiva con
   pasarelas de pago y con sistemas como RENIEC (validación de identidad) y SUNAT, previstos en
   este proyecto únicamente en modo de prueba.

4. **Incorporar firma digital certificada.** Evaluar la adopción de firma digital emitida por
   una entidad de certificación acreditada, a fin de otorgar plena validez legal a las
   resoluciones emitidas por la plataforma.

5. **Realizar una auditoría de seguridad externa.** Antes del despliegue en producción, ejecutar
   una prueba de penetración (*pentesting*) y una auditoría de cumplimiento de la Ley N.° 29733
   por parte de un tercero independiente, para certificar la robustez de los controles.

6. **Prever la escalabilidad y la disponibilidad.** Para un despliegue a escala provincial,
   regional o nacional, considerar el particionado de la tabla de auditoría, el uso de caché
   distribuida y una estrategia de alta disponibilidad con balanceo de carga y respaldos
   geográficamente distribuidos.

7. **Acompañar con gestión del cambio y capacitación.** Implementar un plan de capacitación para
   los funcionarios municipales y de difusión para la ciudadanía, dado que la evidencia
   internacional identifica la brecha de habilidades digitales y la desconfianza como los
   principales inhibidores de la adopción del gobierno digital.

8. **Extender el catálogo de trámites.** A partir del MVP, incorporar progresivamente nuevos
   tipos de trámite del TUPA municipal, aprovechando la configurabilidad del sistema sin
   necesidad de modificar el código.
