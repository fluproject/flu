<?php 
session_start();

//Comprobamos que existe una sesi�n y tiene la variable conectado, si no, redirige a la p�gina principal
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

#Almacenamos en variables el usuario y contrase�a ingresados en el formulario
$user = strip_tags(filtrar($_POST["user"]));
$pass = sha1(strip_tags(filtrar($_POST["pass"])));

#Realizamos la conexi�n a la BBDD
include "conexion-bbdd.php";

$result=mysql_query("INSERT INTO t_usuarios VALUES ('".$user."','".$pass."')",$conexion);

include "cerrar-conexion-bbdd.php";

Header("Location: gestionar-usuarios.php");
?>