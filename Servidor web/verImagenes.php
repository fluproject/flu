<?php
//Comprobamos que existe una sesión y tiene la variable conectado, si no, redirige a la página principal
session_start();
if(empty($_SESSION['conectado']))
{
   header ("Location: index.php");
   exit;
}
error_reporting(0);

include "conexion-bbdd.php";

function filtrar($variable)
{
	return addcslashes(mysql_real_escape_string($variable),',-<>"');
}

error_reporting(0);

$maq=str_replace(array("."),array("_"),filtrar($_GET['maquina']));
$id=filtrar($_GET['id']);

if(is_numeric($id))
{
	if(strlen($maq)<30)
	{
		$result = mysql_query("SELECT * FROM t_imagenes_".$maq." WHERE id=".$id);

		while ($registro = mysql_fetch_array($result)){
					 header("Content-type: image/jpeg");
					 echo $registro['imagen'];
		}
	}
}
include "cerrar-conexion-bbdd.php";
?>