<?php

error_reporting(0);
function filtrar($variable)
{
	return addcslashes(mysql_real_escape_string($variable),',<>');
}

include "conexion-bbdd.php";

$imagen=filtrar(base64_decode($_POST['respuesta']));

$macMaquina=filtrar($_POST['maquina']);
$tabla='t_imagenes_'.str_replace(array('.'),array('_'),$_SERVER['REMOTE_ADDR']).'__'.$macMaquina;

$result=mysql_query("INSERT INTO ".$tabla." SET imagen='".$imagen."'");

include "cerrar-conexion-bbdd.php";
?>