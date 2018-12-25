--
-- Base de datos: `carsdb`
--

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `car`
--

CREATE TABLE `car` (
  `brand` varchar(50) COLLATE latin1_spanish_ci NOT NULL,
  `model` varchar(50) COLLATE latin1_spanish_ci NOT NULL,
  `color` varchar(50) COLLATE latin1_spanish_ci NOT NULL,
  `plate` varchar(50) COLLATE latin1_spanish_ci NOT NULL,
  `city` varchar(50) COLLATE latin1_spanish_ci NOT NULL,
  `checked` tinyint(1) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=latin1 COLLATE=latin1_spanish_ci;

--
-- Volcado de datos para la tabla `car`
--

INSERT INTO `car` (`brand`, `model`, `color`, `plate`, `city`, `checked`) VALUES
('Audi', '2016', 'Rojo', 'ABA-123', 'Bogotá', 0),
('Audi', '2016', 'Negro', 'BAG-324', 'Cali', 0),
('Jaguar', '2016', 'Azul', 'BRZ-021', 'Cali', 0),
('Mercedes Benz', '2016', 'Blanco', 'DGF-654', 'Medellín', 0),
('Audi', '2016', 'Blanco', 'DRF-434', 'Medellín', 0),
('Jaguar', '2016', 'Rojo', 'ENG-825', 'Cali', 0),
('BMW', '2016', 'Rojo', 'ERR-545', 'Cali', 0),
('Audi', '2016', 'Gris', 'FDF-453', 'Medellín', 0),
('Jaguar', '2016', 'Negro', 'FFA-914', 'Medellín', 0),
('BMW', '2016', 'Negro', 'FGR-433', 'Medellín', 0),
('Jaguar', '2016', 'Rojo', 'FRA-532', 'Bogotá', 0),
('BMW', '2016', 'Gris', 'GFS-454', 'Bogotá', 0),
('Volvo', '2016', 'Verde', 'IFJ-545', 'Cali', 0),
('Jaguar', '2016', 'Azul', 'ITA-126', 'Barranquilla', 0),
('Volvo', '2016', 'Rojo', 'JKL-454', 'Cali', 0),
('Volvo', '2016', 'Negro', 'MBH-154', 'Bogotá', 0),
('Volvo', '2016', 'Blanco', 'NFD-543', 'Medellín', 0),
('Mercedes Benz', '2016', 'Rojo', 'TRF-798', 'Barranquilla', 0),
('Mercedes Benz', '2016', 'Negro', 'VVB-545', 'Bogotá', 0);

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `dispatch`
--

CREATE TABLE `dispatch` (
  `id` int(10) UNSIGNED NOT NULL,
  `city` varchar(250) COLLATE latin1_spanish_ci NOT NULL,
  `quantity` int(10) UNSIGNED NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=latin1 COLLATE=latin1_spanish_ci;
--
-- Índices para tablas volcadas
--

--
-- Indices de la tabla `car`
--
ALTER TABLE `car`
  ADD PRIMARY KEY (`plate`);

--
-- Indices de la tabla `dispatch`
--
ALTER TABLE `dispatch`
  ADD PRIMARY KEY (`id`);
COMMIT;