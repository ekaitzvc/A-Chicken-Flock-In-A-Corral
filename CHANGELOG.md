# CHANGELOG

## [Unreleased]
### Added
- InDev objeto para probar cosas:
	- Generacion random de gallinas.
		- Se generan como "Egg" para ver como crecen.
- Sistema base de genes y arquitectura de la gallina.
	- Comportamiento y definicion de genes.
	- Generacion Random de genes en gallinas nuevas.
- Genes: LocusE, LocusC, LocusS, LocusBL

- Caracteristica "GenComplexityRandRate" que permite una tendencia a alelos "normales" en la generación de genes random, evitando alelos extremadamente complejos.
"Solo se ha implementado la variable, pues aun no es funcional".

- Los nombres de los alelos muestran su nombre genetico para facilitar su identificación.

### Changed
- Gen: LocusS ahora es "SexLinked", como ocurre el en la vida real.