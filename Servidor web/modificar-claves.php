<?php 
//Comprobamos que existe una sesión y tiene la variable conectado, si no, redirige a la página principal
session_start();
if(empty($_SESSION['conectado']))
{
   header ("Location: index.php");
   exit;
}
error_reporting(0);

function filtrar($variable)
{
	return addcslashes(mysql_real_escape_string($variable),',<>');
}

#Almacenamos en variables el usuario y contraseña ingresados en el formulario
$key = filtrar($_POST["key"]);
$iv = filtrar($_POST["iv"]);

#Realizamos la conexión a la BBDD
include "conexion-bbdd.php";

#Si el valor no es vacio, se modifica
if((strlen($key)>0)and(strlen($iv)>0))
{
	$result=mysql_query("DELETE from t_crypt",$conexion);
	$result=mysql_query("INSERT INTO t_crypt VALUES ('" . $key . "','". $iv ."')",$conexion);
}
include "cerrar-conexion-bbdd.php";

Header("Location: configurar-claves.php");
?>