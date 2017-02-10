-- phpMyAdmin SQL Dump
-- version 3.3.9
-- http://www.phpmyadmin.net
--
-- Servidor: localhost
-- Tiempo de generación: 27-04-2012 a las 16:36:19
-- Versión del servidor: 5.5.8
-- Versión de PHP: 5.3.5

SET SQL_MODE="NO_AUTO_VALUE_ON_ZERO";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8 */;

--
-- Base de datos: `flubbdd`
--
CREATE DATABASE `flubbdd` DEFAULT CHARACTER SET latin1 COLLATE latin1_swedish_ci;
USE `flubbdd`;

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `t_captcha`
--

CREATE TABLE IF NOT EXISTS `t_captcha` (
  `captcha` varchar(5) DEFAULT NULL
) ENGINE=MyISAM DEFAULT CHARSET=latin1;

--
-- Volcar la base de datos para la tabla `t_captcha`
--


-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `t_crypt`
--

CREATE TABLE IF NOT EXISTS `t_crypt` (
  `key` varchar(100) DEFAULT NULL,
  `iv` varchar(100) DEFAULT NULL
) ENGINE=MyISAM DEFAULT CHARSET=latin1;

--
-- Volcar la base de datos para la tabla `t_crypt`
--

INSERT INTO `t_crypt` (`key`, `iv`) VALUES
('qwertyuioplkjhgf', 'qwertyuioplkjhgf');

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `t_maquinas`
--

CREATE TABLE IF NOT EXISTS `t_maquinas` (
  `maquina` varchar(200) DEFAULT NULL,
  `actualizacion` datetime DEFAULT NULL,
  `sistema` varchar(20) DEFAULT NULL
) ENGINE=MyISAM DEFAULT CHARSET=latin1;

--
-- Volcar la base de datos para la tabla `t_maquinas`
--


-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `t_usuarios`
--

CREATE TABLE IF NOT EXISTS `t_usuarios` (
  `user` varchar(50) NOT NULL DEFAULT '',
  `password` varchar(40) DEFAULT NULL,
  PRIMARY KEY (`user`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;

--
-- Volcar la base de datos para la tabla `t_usuarios`
--

INSERT INTO `t_usuarios` (`user`, `password`) VALUES
('admin', '7110eda4d09e062aa5e4a390b0a572ac0d2c0220');
