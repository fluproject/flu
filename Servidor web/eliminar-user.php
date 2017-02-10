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
$user = strip_tags(filtrar($_GET["u"]));

#Realizamos la conexión a la BBDD
include "conexion-bbdd.php";

#Evita que se pueda eliminar el usuario actual
if($user!=filtrar($_SESSION['user_con']))
{
	$result=mysql_query("DELETE FROM t_usuarios WHERE user='".$user."'",$conexion);
}
include "cerrar-conexion-bbdd.php";

Header("Location: gestionar-usuarios.php");
?>