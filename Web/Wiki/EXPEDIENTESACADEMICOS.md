# Changelog
Todos los cambios a este Proyecto serán documentados en este archivo.

El formato se basa en [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
y este Proyecto se adhiere a [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [UNRELEASED]
[UNRELEASED]: https://dev.azure.com/unirnet/UNIR/_git/ExpedientesErpNetCore/branchCompare?baseVersion=GT3.1.0&targetVersion=GBdevelop&_a=commits
## **Added**
- [x] [Product Backlog Item 331068: Añadir una marca que indique que se ha realizado la migración de calificaciones con un expediente existente](https://dev.azure.com/unirnet/UNIR/_workitems/edit/331068)
- [x] [Product Backlog Item 333662: Modificar el logotipo de UNIR](https://dev.azure.com/unirnet/UNIR/_workitems/edit/333662)
- [x] [Product Backlog Item 326491: Migración de calificaciones de los expedientes existentes](https://dev.azure.com/unirnet/UNIR/_workitems/edit/326491)
- [x] **[CONFIG]**[Product Backlog Item 315212: Obtener calificaciones de mensaje rabbit NotaFinalGenerada](https://dev.azure.com/unirnet/UNIR/_workitems/edit/315212)
- [x] [Product Backlog Item 322448: Consultar las calificaciones del Expediente - añadir reconocimientos](https://dev.azure.com/unirnet/UNIR/_workitems/edit/322448)
- [x] [Product Backlog Item 325968: Health check Expedientes ERP](https://dev.azure.com/unirnet/UNIR/_workitems/edit/325968)
- [x] [Product Backlog Item 332389: Guardar resultado de la migración de calificaciones de los expedientes existentes](https://dev.azure.com/unirnet/UNIR/_workitems/edit/332389)
- [x] [Product Backlog Item 332794: Migración de calificaciones de los expedientes existentes: añadir relación entre expedientes y asignaturas](https://dev.azure.com/unirnet/UNIR/_workitems/edit/332794)
- [x] [Product Backlog Item 340051: En el Listado de Expedientes, modificar la forma de localizar un alumno para no penalizar el rendimiento, así como la búsqueda por estudio o plan](https://dev.azure.com/unirnet/UNIR/_workitems/edit/340051)
- [x] [Product Backlog Item 340052: En el Listado de Seguimiento de Expedientes, modificar la forma de localizar un seguimiento para no penalizar el rendimiento, así como la búsqueda por estudio o plan](https://dev.azure.com/unirnet/UNIR/_workitems/edit/340052)
- [x] [Product Backlog Item 308130: Crear Solicitudes de Expedición de Títulos desde Expedientes](https://dev.azure.com/unirnet/UNIR/_workitems/edit/308130)
- [x] [Product Backlog Item 346272: Refactorizar formulario de seguimientos del expediente](https://dev.azure.com/unirnet/UNIR/_workitems/edit/346272)
- [x] [Product Backlog Item 345358: Actualizar el origen externo en la tabla de seguimientos de expedientes en todos los mensajes rabbits a los que se suscribe la aplicación de Expedientes](https://dev.azure.com/unirnet/UNIR/_workitems/edit/345358)
- [x] [Product Backlog Item 347965: Limitar que se seleccionen como máximo 100 registros al crear la solicitud](https://dev.azure.com/unirnet/UNIR/_workitems/edit/347965)

### Dependencies
- [ ][**ERP Matriculación**] [Product Backlog Item 329163: Se necesita genera un nuevo end point para obtener las asignaturas matriculadas a través del idRefCurso y de idsAlumnos](https://dev.azure.com/unirnet/UNIR/_workitems/edit/329163)
- [ ][**ERP Matriculación**] [Product Backlog Item 329637: Crear Servicios para listar las asignaturas ofertadas por los ids asignaturas plan](https://dev.azure.com/unirnet/UNIR/_workitems/edit/329637)

## Fixed
- [x] [Bug 344684: No está funcionando el filtro de usuarios en el listado de seguimientos](https://dev.azure.com/unirnet/UNIR/_workitems/edit/344684)
- [x] [Bug 346886: Al crear el Expediente no se guarda la hora en el seguimiento](https://dev.azure.com/unirnet/UNIR/_workitems/edit/346886)
- [x] [Bug 346887: Al actualizar el Expediente no se guarda la hora en el seguimiento](https://dev.azure.com/unirnet/UNIR/_workitems/edit/346887)

## [3.1.0] [PRE 08/02/2023]
[3.1.0]: https://dev.azure.com/unirnet/UNIR/_git/ExpedientesErpNetCore/branchCompare?baseVersion=GT3.0.0&targetVersion=GT3.1.0&_a=commits
## **Added**

- [x] [Product Backlog Item 328906: BBDD: Crear nueva tabla de relación entre asignaturas y eliminar campo de relación de asignatura en [Expedientes].[dbo].[AsignaturasExpedientes]](https://dev.azure.com/unirnet/UNIR/_workitems/edit/328906)
- [x] [Product Backlog Item 326189: Para un usuario con rol de consulta, la pantalla "Consolidar requisitos asociados al expediente" debe de ser de sólo consulta y no poder gestionar nada](https://dev.azure.com/unirnet/UNIR/_workitems/edit/326189)
- [x] [Product Backlog Item 283843: Modificar mensaje Rabbit MatriculaDesestimada para actualizar el expediente](https://dev.azure.com/unirnet/UNIR/_workitems/edit/283843)
## [3.0.0] [PRE 23/01/2023]
[3.0.0]: https://dev.azure.com/unirnet/UNIR/_git/ExpedientesErpNetCore/branchCompare?baseVersion=GT2.2.1&targetVersion=GT3.0.0&_a=commits
## **Added**

### Vista Expediente
- [x] [Product Backlog Item 310455: Modificar tamaño de la foto del alumno en la consulta de alumno, expedientes, matrículas, condiciones y requisitos](https://dev.azure.com/unirnet/UNIR/_workitems/edit/310455)
- [x] [Product Backlog Item 289251: Consultar las calificaciones del Expediente](https://dev.azure.com/unirnet/UNIR/_workitems/edit/289251)
- [x] [Product Backlog Item 285982: Resumen del Expediente - resumen de créditos](https://dev.azure.com/unirnet/UNIR/_workitems/edit/285982)
- [x] [Product Backlog Item 285964: Resumen del Expediente - estado y situación](https://dev.azure.com/unirnet/UNIR/_workitems/edit/285964)
- [x] [Product Backlog Item 285981: Resumen del Expediente - cronología](https://dev.azure.com/unirnet/UNIR/_workitems/edit/285981)
- [x] [Product Backlog Item 285980: Resumen del Expediente - plan de estudio](https://dev.azure.com/unirnet/UNIR/_workitems/edit/285980)
- [x] [Product Backlog Item 289320: Establecer las opciones de un expediente en el Listado de Expedientes](https://dev.azure.com/unirnet/UNIR/_workitems/edit/289320)
- [x] [Product Backlog Item 305804: Modificar Listado de Expedientes para incluir el estado y la situación del expediente y permitir la búsqueda directa por identificador de integración de alumno e identificador de plan](https://dev.azure.com/unirnet/UNIR/_workitems/edit/305804)
- [x] [Product Backlog Item 307607: Poner cabecera en la edición del expediente, puede titularse, anotaciones y seguimiento](https://dev.azure.com/unirnet/UNIR/_workitems/edit/307607)
- [x] [Product Backlog Item 305812: Modificar columnas del listado de datos financieros del alumno](https://dev.azure.com/unirnet/UNIR/_workitems/edit/305812)
- [x] [Product Backlog Item 306008: Crear Servicio para crear la información de Tipo Situacion Estado del expediente](https://dev.azure.com/unirnet/UNIR/_workitems/edit/306008)
- [x] [Product Backlog Item 308051: Crear Servicio para asociar las asignaturas al expediente](https://dev.azure.com/unirnet/UNIR/_workitems/edit/308051)
- [x] [Product Backlog Item 315443: Establecer criterio para conocer cuándo una matrícula es la primera matrícula](https://dev.azure.com/unirnet/UNIR/_workitems/edit/315443)

### Requisitos
- [x] [Product Backlog Item 301379: Crear requisitos de comportamientos asociados en la consolidación de requisitos asociados al expediente](https://dev.azure.com/unirnet/UNIR/_workitems/edit/301379)
- [x] [Product Backlog Item 276717: Consolidar requisitos asociados al expediente](https://dev.azure.com/unirnet/UNIR/_workitems/edit/276717)
- [x] [Product Backlog Item 316446: No se actualiza el dato Orden del requisito y modificación de la tabla RequisitosExpedientesFileType](https://dev.azure.com/unirnet/UNIR/_workitems/edit/316446)
- [x] [Product Backlog Item 320487: Al consultar los requisitos en la ficha del alumno, el sistema debe navegar a la consolidación de dicho requisito y no a la gestión del requisito](https://dev.azure.com/unirnet/UNIR/_workitems/edit/320487)
- [x] [Product Backlog Item 314453: Modificar la consolidación de requisitos asociados al expediente (por redacción desacertada del PBI asociado) y permitir siempre la eliminación de la consolidación de un requisito](https://dev.azure.com/unirnet/UNIR/_workitems/edit/314453)

### Rabbit
- [x] [Product Backlog Item 283902: Modificar mensaje Rabbit MatriculaVariacionRecuperada para actualizar el expediente](https://dev.azure.com/unirnet/UNIR/_workitems/edit/283902)
- [x] [Product Backlog Item 283899: Modificar mensaje Rabbit MatriculaVariacionDesestimada para actualizar el expediente](https://dev.azure.com/unirnet/UNIR/_workitems/edit/283899)
- [x] [Product Backlog Item 283694: Modificar mensaje Rabbit MatriculaRealizada para asociar asignaturas al expediente](https://dev.azure.com/unirnet/UNIR/_workitems/edit/283694)
- [x] [Product Backlog Item 283506: Modificar mensaje Rabbit PrematriculaCompletada para asociar la matrícula al expediente](https://dev.azure.com/unirnet/UNIR/_workitems/edit/283506)
- [x] [Product Backlog Item 275438: Modificar mensaje Rabbit PrematriculaCompletada para crear las consolidación de los requisitos en el expediente](https://dev.azure.com/unirnet/UNIR/_workitems/edit/275438)
- [x] [Product Backlog Item 283846: Modificar mensaje Rabbit MatriculaReiniciada para actualizar el expediente](https://dev.azure.com/unirnet/UNIR/_workitems/edit/283846)
- [x] [Product Backlog Item 283849: Modificar mensaje Rabbit MatriculaRecuperada para actualizar el expediente](https://dev.azure.com/unirnet/UNIR/_workitems/edit/283849)
- [x] [Product Backlog Item 283893: Modificar mensaje Rabbit MatriculaAmpliacionRealizada para asociar asignaturas al expediente](https://dev.azure.com/unirnet/UNIR/_workitems/edit/283893)
- [x] [Product Backlog Item 283848: Modificar mensaje Rabbit MatriculaAnulada para actualizar el expediente](https://dev.azure.com/unirnet/UNIR/_workitems/edit/283848)
- [x] [Product Backlog Item 283894: Modificar mensaje Rabbit MatriculaAmpliacionDesestimada para actualizar el expediente](https://dev.azure.com/unirnet/UNIR/_workitems/edit/283894)
- [x] [Product Backlog Item 283895: Modificar mensaje Rabbit MatriculaAmpliacionReiniciada para actualizar el expediente](https://dev.azure.com/unirnet/UNIR/_workitems/edit/283895)
- [x] [Product Backlog Item 283896: Modificar mensaje Rabbit MatriculaAmpliacionAnulada para actualizar el expediente](https://dev.azure.com/unirnet/UNIR/_workitems/edit/283896)
- [x] [Product Backlog Item 283898: Modificar mensaje Rabbit MatriculaVariacionRealizada para asociar asignaturas al expediente](https://dev.azure.com/unirnet/UNIR/_workitems/edit/283898)
- [x] [Product Backlog Item 283900: Modificar mensaje Rabbit MatriculaVariacionReiniciada para actualizar el expediente](https://dev.azure.com/unirnet/UNIR/_workitems/edit/283900)
- [x] [Product Backlog Item 283901: Modificar mensaje Rabbit MatriculaVariacionAnulada para actualizar el expediente](https://dev.azure.com/unirnet/UNIR/_workitems/edit/283901)
- [x] [Product Backlog Item 327168: Modificar mensaje Rabbit MatriculaVariacionRealizada: la situación del expediente es la misma, con independencia del estado](https://dev.azure.com/unirnet/UNIR/_workitems/edit/327168)

### Requisitos
- [x] [Product Backlog Item 301379: Crear requisitos de comportamientos asociados en la consolidación de requisitos asociados al expediente](https://dev.azure.com/unirnet/UNIR/_workitems/edit/301379)
- [x] [Product Backlog Item 276717: Consolidar requisitos asociados al expediente](https://dev.azure.com/unirnet/UNIR/_workitems/edit/276717)

### Modelado
- [x] [Product Backlog Item 304105: Añadir tipo de expediente en la tabla de consolidación de requisitos asociados al expediente](https://dev.azure.com/unirnet/UNIR/_workitems/edit/304105)
- [x] [Product Backlog Item 304987: Configuración de expedientes por universidad para poder guardar documentos en la consolidación requisitos asociados al expediente](https://dev.azure.com/unirnet/UNIR/_workitems/edit/304987)
- [x] [Product Backlog Item 306050: Añadir foreing key en la consolidación del requisito contra la causa asociada al estado](https://dev.azure.com/unirnet/UNIR/_workitems/edit/306050)
- [x] [Product Backlog Item 302182: Modificar tabla que permite asociar asignaturas al expediente y actualizar tabla de tipo de situación](https://dev.azure.com/unirnet/UNIR/_workitems/edit/302182)
### Optimizaciones
- [x] [Product Backlog Item 314994: Lint Expedientes (securización cliente)](https://dev.azure.com/unirnet/UNIR/_workitems/edit/314994)
- [x] [Product Backlog Item 306857: Securización de Aplicación de Expedientes](https://dev.azure.com/unirnet/UNIR/_workitems/edit/306857)

## **Dependencies**
- [ ][**ERP Matriculación**] [Product Backlog Item 309479: Crear servicio para obtener las matrículas por IdRefExpedienteAlumno](https://dev.azure.com/unirnet/UNIR/_workitems/edit/309479)
- [ ][**ERP Matriculación**] [Product Backlog Item 315661: Obtener Tipo y Estado de la matrícula por su id de integración](https://dev.azure.com/unirnet/UNIR/_workitems/edit/315661)
- [ ][**ERP Matriculación**] [Product Backlog Item 323353: Modificación de servicio api/v1/Alumnos/integracion/{idIntegracion}/matriculas-documentos para añadir asignaturas de matriculas](https://dev.azure.com/unirnet/UNIR/_workitems/edit/323353)
- [ ][**ERP Plan Estudio**] [Product Backlog Item 321015: Obtener más datos de asignaturas en el servicio del grafo del plan](https://dev.azure.com/unirnet/UNIR/_workitems/edit/321015)
- [ ][**ERP Plan Estudio**] [Product Backlog Item 314590: Obtener requerimientos paginados por el id del plan](https://dev.azure.com/unirnet/UNIR/_workitems/edit/314590)

## Fixed
- [x] [Bug 315734: Falla al guardar la vía de acceso en el expediente, pero actualiza igualmente en el ERP Académico, dando un error de validación que no es tal, RELACIÓN CON BUG 316965 ERP ACADÉMICO](https://dev.azure.com/unirnet/UNIR/_workitems/edit/315734)


## [2.2.1] [PRO 17/10/2022] [PRE 13/10/2022]
### Fixed
- [x][Bug 303559: Errores en Listado de Expedientes (sincronización con ERP-Academico)](https://dev.azure.com/unirnet/UNIR/_workitems/edit/303559)

## [2.2.0]  [PRO 17/10/2022] [PRE 10/10/2022]
### Added
- [x] [Product Backlog Item 298243: Corrección de la gestión de los documentos en los requisitos de expediente](https://dev.azure.com/unirnet/UNIR/_workitems/edit/298243)
### Fixed
- [x][Bug 300606: No sincroniza correctamente la información entre alumno-expedientes-matrículas](https://dev.azure.com/unirnet/UNIR/_workitems/edit/300606)

## Dependencies

- [x][**ERP Matriculación**]  [Product Backlog Item 300471: Obtener documentos alumnos del servicio datos-basicos](https://dev.azure.com/unirnet/UNIR/_workitems/edit/300471)
- [x][**ERP Matriculación**]  [Product Backlog Item 301113: Creación de Servicio api/v1/alumno/integracion/{idIntegracion}/matriculas-documentos](https://dev.azure.com/unirnet/UNIR/_workitems/edit/301113)

## [2.1.0] [PRO 17/10/2022] [PRE 22/09/2022]
### Added
- [x] [Product Backlog Item 290731: Añadir tabla en la ampliación del modelo de datos para dar cabida a requisitos y comportamientos en el expediente](https://dev.azure.com/unirnet/UNIR/_workitems/edit/290731)
- [x] [Product Backlog Item 272696: Listado de requisitos para expedientes](https://dev.azure.com/unirnet/UNIR/_workitems/edit/272696)
- [x] [Product Backlog Item 273067: Listado de comportamientos de requisitos de expediente](https://dev.azure.com/unirnet/UNIR/_workitems/edit/273067)
- [x] [Product Backlog Item 272697: Gestión de requisitos de expediente](https://dev.azure.com/unirnet/UNIR/_workitems/edit/272697)
- [x] **[CONFIG]** [Product Backlog Item 275646: Modificar Consultar alumno, expedientes y matrículas, además de todas las condiciones, para mostrar si el alumno tiene o no saldo pendiente](https://dev.azure.com/unirnet/UNIR/_workitems/edit/275646)
- [x] [Product Backlog Item 276705: Ampliar Consultar requisitos asociadas al alumno, expedientes y matrículas para incluir los requisitos del expediente y el estado de las condiciones de matrículas](https://dev.azure.com/unirnet/UNIR/_workitems/edit/276705)
- [x] [Product Backlog Item 273519: Gestión de comportamientos de requisitos de expediente](https://dev.azure.com/unirnet/UNIR/_workitems/edit/273519)
### Changed 
- [x] **[SCRIPT]** [Product Backlog Item 265612: Consultar alumno, expedientes y matrículas, además de todas las condiciones](https://dev.azure.com/unirnet/UNIR/_workitems/edit/265612)
### Dependencies
- [ ][**ERP Matriculación**] [Product Backlog Item 297376: Obtener IdRefExpedienteAlumno de las matriculas](https://dev.azure.com/unirnet/UNIR/_workitems/edit/297376)


## [2.0.0] [PRO 17/10/2022] [PRE 22/09/2022]
### Added
- [x] [Product Backlog Item 285062: Modificar base de datos de Expedientes para poder asociar sus asignaturas](https://dev.azure.com/unirnet/UNIR/_workitems/edit/285062)
- [x] [Product Backlog Item 267951: Consultar condiciones asociadas al alumno, expedientes y matrículas](https://dev.azure.com/unirnet/UNIR/_workitems/edit/267951)
- [x] [Product Backlog Item 272695: Ampliación del modelo de datos para dar cabida a requisitos y comportamientos en el expediente](https://dev.azure.com/unirnet/UNIR/_workitems/edit/272695)
- [x] **[CONFIG]** [Product Backlog Item 279365: Actualizar Rutas de endpoint en Expedientes y ERP Académico](https://dev.azure.com/unirnet/UNIR/_workitems/edit/279365)
- [x] [Product Backlog Item 265612: Consultar alumno, expedientes y matrículas, además de todas las condiciones](https://dev.azure.com/unirnet/UNIR/_workitems/edit/265612)
- [x] [Product Backlog Item 284452: No guardar IdRefTipoVinculacion cuando se cree o edite un expediente desde ERP](https://dev.azure.com/unirnet/UNIR/_workitems/edit/284452)


### Changed 
- [x] [Product Backlog Item 275290: Modificar Editar/Mostrar Matrícula para que, al gestionar o consultar, la información de la titulación de acceso sea la misma que en el expediente](https://dev.azure.com/unirnet/UNIR/_workitems/edit/275290)



## [1.5.0] [PRO 06/07/2022] [PRE 06/07/2022]
### Added
- [x] [Product Backlog Item 267916: Modificar mensaje Rabbit MatriculaRealizada para establecer estado del expediente](https://dev.azure.com/unirnet/UNIR/_workitems/edit/267916)
- [x] [Product Backlog Item 268810: Modificar el Listado de Expedientes para mostrar información de los títulos/certificados del expediente seleccionado](https://dev.azure.com/unirnet/UNIR/_workitems/edit/268810)
- [x] [Product Backlog Item 267921: Establecer relaciones entre expedientes existentes](https://dev.azure.com/unirnet/UNIR/_workitems/edit/267921)
- [x] [Product Backlog Item 279492: Eliminar IdViaAcesso en el back y renombrar en el front (Partial), corregir en front y back](https://dev.azure.com/unirnet/UNIR/_workitems/edit/279492)
- [x] [Product Backlog Item 279748: Exponer servicio para obtener la información de Titulacion de Acceso y los Ids Ref de Especializaciones asociados a un Expediente](https://dev.azure.com/unirnet/UNIR/_workitems/edit/279748)
- [x] [Product Backlog Item 279613: Manejo de lista de Errores personalizados en la vista(Expedientes)](https://dev.azure.com/unirnet/UNIR/_workitems/edit/279613)
- [x] [Product Backlog Item 280798: Exponer servicio para obtener el primer parámetro de configuración de los expedientes](https://dev.azure.com/unirnet/UNIR/_workitems/edit/280798)
- [x] [Product Backlog Item 283217: Modificar servicios de "tipo-vinculación" y "query-no-paged"](https://dev.azure.com/unirnet/UNIR/_workitems/edit/283217)
- [x] [Product Backlog Item 281106: Ajustar servicio expuesto que Modifica Expediente desde ERP para que devuelva un valor booleano que indique si se han añadido seguimientos al modificar la Versión Plan, Vía de Acceso o Titulación de Acceso](https://dev.azure.com/unirnet/UNIR/_workitems/edit/281106)


## [1.4.0] [PRE 29/06/2022]
### Fixed
- [ ] [Bug 275013: Error en el combobox de institución docente](https://dev.azure.com/unirnet/UNIR/_workitems/edit/275013)

### Added
- [x] [Product Backlog Item 173093: Editar la información relacionada con la titulación de acceso en el expediente](https://dev.azure.com/unirnet/UNIR/_workitems/edit/173093)
- [x] [Product Backlog Item 259971: Al crear el expediente, el registro del mismo debe incluir los códigos de estudio y plan de estudio en el dato del nombre correspondiente](https://dev.azure.com/unirnet/UNIR/_workitems/edit/259971)
- [x] [Product Backlog Item 265027: Editar la información relacionada con la titulación de acceso en el expediente con respecto a la geolocalización](https://dev.azure.com/unirnet/UNIR/_workitems/edit/265027)
- [ ] [Product Backlog Item 264239: Actualizar a la versión 13 de Expedientes Académicos](https://dev.azure.com/unirnet/UNIR/_workitems/edit/264239)
- [x] [Product Backlog Item 269116: SUPERCAP: Modificar servicio expedientes-alumnos/expediente-alumno para ajustarse a los datos del Gestor](https://dev.azure.com/unirnet/UNIR/_workitems/edit/269116)
- [x] [Product Backlog Item 271467: Exponer Servicio para actualizar la fecha de Apertura de un Expediente](https://dev.azure.com/unirnet/UNIR/_workitems/edit/271467)
- [x] [Product Backlog Item 235866: Añadir componente de Anotaciones (Bitácora) para Observaciones en la Edición de Expediente](https://dev.azure.com/unirnet/UNIR/_workitems/edit/235866)
- [x] [Product Backlog Item 265708: Relacionar expedientes y establecer su estado, y actualizar hitos de los expedientes](https://dev.azure.com/unirnet/UNIR/_workitems/edit/265708)
- [x] [Product Backlog Item 272632: Eliminar columna IdRefAlumno de la Tabla ExpedienteAlumno](https://dev.azure.com/unirnet/UNIR/_workitems/edit/272632)
- [x] [Product Backlog Item 274464: Modificar Servicio de Crear y Editar expediente para que reciba todos los parámetros enviados desde ERP.](https://dev.azure.com/unirnet/UNIR/_workitems/edit/274464)
- [x] [Product Backlog Item 270037: Roles de Expedientes](https://dev.azure.com/unirnet/UNIR/_workitems/edit/270037)
- [x] [Product Backlog Item 269065: Modificar la edición de la institución en la titulación de acceso del expediente y modificar la etiqueta de uno de los campos solicitados](https://dev.azure.com/unirnet/UNIR/_workitems/edit/269065)
- [x] [Product Backlog Item 267912: Modificar mensaje Rabbit PrematriculaCompletada para actualizar estado del expediente y establecer la relación de cambio de plan con otro expediente](https://dev.azure.com/unirnet/UNIR/_workitems/edit/267912)
- [x] [Product Backlog Item 267925: Establecer el estado y los hitos/especializaciones de todos los expedientes con los valores correspondientes](https://dev.azure.com/unirnet/UNIR/_workitems/edit/267925)

### Changed 
- [x] [Product Backlog Item 267401: SUPERCAP: Crear servicio que recupere datos del Expediente](https://dev.azure.com/unirnet/UNIR/_workitems/edit/267401)

## [1.3.1] [PRO 25/05/2022] [PRE 24/05/2022]
### Added
- [x] [Product Backlog Item 267401: SUPERCAP: Crear servicio que recupere datos del Expediente](https://dev.azure.com/unirnet/UNIR/_workitems/edit/267401)

## [1.3.0] [PRE 28/04/2022]
### Added
- [x] [Product Backlog Item 256404: Modificar columnas en el Listado de Expedientes](https://dev.azure.com/unirnet/UNIR/_workitems/edit/256404)
- [x] [Product Backlog Item 256400: Hay que actualizar la tabla de seguimiento al modificar los datos del expediente](https://dev.azure.com/unirnet/UNIR/_workitems/edit/256400)

## [1.2.1] [PRE 17/04/2022]
### Changed
- [x] [Bug 258644: El campo Nro de semestres debe permitir ingresar solo valores numéricos](https://dev.azure.com/unirnet/UNIR/_workitems/edit/258644)

## [1.2.0] [PRE 11/04/2022]
### Changed
- [x] [Product Backlog Item 241774: Crear Servicio para modificar la Titulación de Acceso del expediente](https://dev.azure.com/unirnet/UNIR/_workitems/edit/241774)
- [x] [Product Backlog Item 240744: Modificar Listado de Seguimiento de Expedientes](https://dev.azure.com/unirnet/UNIR/_workitems/edit/240744)
- [x] [Product Backlog Item 241777: Crear Servicio para modificar el Vía de acceso del expediente](https://dev.azure.com/unirnet/UNIR/_workitems/edit/241777)

### Fixed
- [x] [Bug 255953: Error en el servicio update-expediente-alumno-partial al Editar un Expediente y no especificar la información de Titulación de Acceso](https://dev.azure.com/unirnet/UNIR/_workitems/edit/255953)

## [1.1.1] [PRE 30/03/2022]
### Added
- [x] [Product Backlog Item 225962: Crear Servicio para modificar el Tipo de vinculación del expediente](https://dev.azure.com/unirnet/UNIR/_workitems/edit/225962)
- [x] [Product Backlog Item 241777: Crear Servicio para modificar el Vía de acceso del expediente](https://dev.azure.com/unirnet/UNIR/_workitems/edit/241777)
- [x] [Product Backlog Item 241774: Crear Servicio para modificar la Titulación de Acceso del expediente](https://dev.azure.com/unirnet/UNIR/_workitems/edit/241774)

### Changed
- [x] [Product Backlog Item 236178: Modificar Puede Titular un Expediente](https://dev.azure.com/unirnet/UNIR/_workitems/edit/236178)
- [x] [Product Backlog Item 240744: Modificar Listado de Seguimiento de Expedientes](https://dev.azure.com/unirnet/UNIR/_workitems/edit/240744)

## [1.1.0] [PRE 29/03/2022]
### Added
- [x] [Product Backlog Item 236112: Migrar esquema ERP_Academico_V2.aca_expediente a la base de datos Expedientes y eliminar las tablas innecesarias](https://dev.azure.com/unirnet/UNIR/_workitems/edit/236112)

### Changed
- [x] [Product Backlog Item 235822: Modificar Edición de un Expediente](https://dev.azure.com/unirnet/UNIR/_workitems/edit/235822)
- [x] [Product Backlog Item 239023: Actualizar el servicio de Editar ExpedientesAlumno](https://dev.azure.com/unirnet/UNIR/_workitems/edit/239023)

### Fixed
- [x] [Bug 251178: No se están actualizando Datos de Expedientes, Titulación y Especializaciones en Expedientes Académicos [Core]](https://dev.azure.com/unirnet/UNIR/_workitems/edit/251178)

## [1.0.1] [PRE 15/03/2022]
### Changed
- [x] [Product Backlog Item 235822: Modificar Edición de un Expediente](https://dev.azure.com/unirnet/UNIR/_workitems/edit/235822)

## [1.0.0]
### Added
- [x] [Product Backlog Item 202906: Cambiar favicon en Expedientes](https://dev.azure.com/unirnet/UNIR/_workitems/edit/202906)
- [x] [Product Backlog Item 203990: Actualización y unificación de componentes genéricos y dependencias en proyectos Angular](https://dev.azure.com/unirnet/UNIR/_workitems/edit/203990)
- [x] [Product Backlog Item 197904: Actualización de Expedientes ERP a .NET 5](https://dev.azure.com/unirnet/UNIR/_workitems/edit/197904)
- [x] [Product Backlog Item 218417: Actualizar todos los Servicios relacionados al negocio de "Puede Titularse" de Expedientes ERP a .NET 5](https://dev.azure.com/unirnet/UNIR/_workitems/edit/218417)
- [x] [Product Backlog Item 231027: Vías de acceso: Modificar la visualización de la información de la vía de acceso al editar la información relacionada con las vías de acceso en el expediente](https://dev.azure.com/unirnet/UNIR/_workitems/edit/231027)
- [x] [Product Backlog Item 235818: Unificar base de datos de Expedientes con esquema de expedientes existente en el ERP Académico](https://dev.azure.com/unirnet/UNIR/_workitems/edit/235818)
- [x] [Product Backlog Item 235819: Modificar Listado de Expedientes](https://dev.azure.com/unirnet/UNIR/_workitems/edit/235819)
- [x] [Product Backlog Item 239020: Actualizar el servicio de Crear ExpedientesAlumno](https://dev.azure.com/unirnet/UNIR/_workitems/edit/239020)
- [x] [Product Backlog Item 239024: Crear servicio para obtener la configuración de la condición 8 del ERP Académico](https://dev.azure.com/unirnet/UNIR/_workitems/edit/239024)
- [x] [Product Backlog Item 239023: Actualizar el servicio de Editar ExpedientesAlumno](https://dev.azure.com/unirnet/UNIR/_workitems/edit/239023)
- [x] [Product Backlog Item 235822: Modificar Edición de un Expediente](https://dev.azure.com/unirnet/UNIR/_workitems/edit/235822)
- [x] [Product Backlog Item 237356: Actualizar aplicaciones .NET 5 a .NET 6](https://dev.azure.com/unirnet/UNIR/_workitems/edit/237356)
- [x] [Product Backlog Item 236178: Modificar Puede Titular un Expediente](https://dev.azure.com/unirnet/UNIR/_workitems/edit/236178)
- [x] [Product Backlog Item 236179: Modificar Seguimiento de un Expediente dentro del Expediente](https://dev.azure.com/unirnet/UNIR/_workitems/edit/236179)

### Fixed
- [x] [Bug 242507: Servicio Expediente-Alumno no devuelve datos de asignaturas superadas](https://dev.azure.com/unirnet/UNIR/_workitems/edit/242507)
- [x] [Bug 242818: Errores no controlados al consultar el servicio Expediente-Alumno](https://dev.azure.com/unirnet/UNIR/_workitems/edit/242818)
- [x] [Bug 240339: Editar expediente no muestra informacion de Version y Via de Acceso](https://dev.azure.com/unirnet/UNIR/_workitems/edit/240339)


[2.2.1]: https://dev.azure.com/unirnet/UNIR/_git/ExpedientesErpNetCore/branchCompare?baseVersion=GT2.2.0&targetVersion=GT2.2.1&_a=commits
[2.2.0]: https://dev.azure.com/unirnet/UNIR/_git/ExpedientesErpNetCore/branchCompare?baseVersion=GT2.1.0&targetVersion=GT2.2.0&_a=commits
[2.1.0]: https://dev.azure.com/unirnet/UNIR/_git/ExpedientesErpNetCore/branchCompare?baseVersion=GT2.0.0&targetVersion=GT2.1.0&_a=commits
[2.0.0]: https://dev.azure.com/unirnet/UNIR/_git/ExpedientesErpNetCore/branchCompare?baseVersion=GT1.5.0&targetVersion=GT2.0.0&_a=commits
[1.5.0]: https://dev.azure.com/unirnet/UNIR/_git/ExpedientesErpNetCore/branchCompare?baseVersion=GT1.4.0&targetVersion=GT1.5.0&_a=commits
[1.4.0]: https://dev.azure.com/unirnet/UNIR/_git/ExpedientesErpNetCore/branchCompare?baseVersion=GT1.3.1&targetVersion=GT1.4.0&_a=commits
[1.3.1]: https://dev.azure.com/unirnet/UNIR/_git/ExpedientesErpNetCore/branchCompare?baseVersion=GT1.3.0&targetVersion=GT1.3.1&_a=commits
[1.3.0]: https://dev.azure.com/unirnet/UNIR/_git/ExpedientesErpNetCore/branchCompare?baseVersion=GT1.2.1&targetVersion=GT1.3.0&_a=commits
[1.2.1]: https://dev.azure.com/unirnet/UNIR/_git/ExpedientesErpNetCore/branchCompare?baseVersion=GT1.2.0&targetVersion=GT1.2.1&_a=commits
[1.2.0]: https://dev.azure.com/unirnet/UNIR/_git/ExpedientesErpNetCore/branchCompare?baseVersion=GT1.1.1&targetVersion=GT1.2.0&_a=commits
[1.1.1]: https://dev.azure.com/unirnet/UNIR/_git/ExpedientesErpNetCore/branchCompare?baseVersion=GT1.1.0&targetVersion=GT1.1.1&_a=commits
[1.1.0]: https://dev.azure.com/unirnet/UNIR/_git/ExpedientesErpNetCore/branchCompare?baseVersion=GT1.0.1&targetVersion=GT1.1.0&_a=commits
[1.0.1]: https://dev.azure.com/unirnet/UNIR/_git/ExpedientesErpNetCore/branchCompare?baseVersion=GT1.0.0&targetVersion=GT1.0.1&_a=commits
[1.0.0]: https://dev.azure.com/unirnet/UNIR/_git/ExpedientesErpNetCore/branchCompare?baseVersion=GTinicial&targetVersion=GT1.0.0&_a=commits