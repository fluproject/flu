<?php 
session_start();

function filtrar($variable)
{
	return addcslashes(mysql_real_escape_string($variable),',<>');
}

//Almacenamos en variables el usuario y contrase�a ingresados en el formulario
$user = filtrar($_POST["user"]);
$pass = sha1(filtrar($_POST["pass"]));
$captcha = filtrar($_POST["captcha"]);

//Realizamos la conexi�n a la BBDD
//Datos de conexi�n a la BBBDD
include "conexion-bbdd.php";

//Descargamos la lista de usuarios (con protecci�n a SQL Injections)
$sql = "SELECT * FROM t_usuarios WHERE user='".($user)."' AND password='".($pass)."'";
$result_a = mysql_query($sql);

//Comprobamos que el captcha sea correcto
$sql2 = "SELECT count(*) FROM t_captcha WHERE captcha='".($captcha)."'";
$result_b = mysql_query($sql2);
if ($row = mysql_fetch_array($result_b))
{
	#Se comprueba que el captcha sea correcto
	if($row[0]>0)
	{
		//Si el usuario y contrase�a son correctos ponemos la variable conectado de la sesi�n a 1 y redirigimos al menuprincipal.php
		if(mysql_fetch_row($result_a))
		{
		   $_SESSION['conectado']=1;
		   $_SESSION['user_con']=$user;
		   Header("Location: menuPrincipal.php");
		}
		//Si el usuario o contrase�a son incorrectos redirigimos a la p�gina principal
		else
		{
			header ("Location: index.php");
			exit;
		}
	}
	else
	{
		header ("Location: index.php");
		exit;
	}
}

include "cerrar-conexion-bbdd.php";
?>