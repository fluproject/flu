<?php

error_reporting(0);
ini_set('max_execution_time', 300);
include "conexion-bbdd.php";

function filtrar($variable)
{
	return addcslashes(mysql_real_escape_string($variable),',/<>"');
}

#Recogemos los valores enviados por POST
$macMaquina = filtrar($_POST['m']);
$comando = filtrar($_POST['comando']);
$id_unico_comando = filtrar($_POST['id']);
$contenidoFichero = $_POST['respuesta'];
$nombre = basename(filtrar($_POST['nombre']));
$formato = basename(filtrar($_POST['formato']));
$sha1 = filtrar($_POST['firmaSHA1']);
$md5 = filtrar($_POST['firmaMD5']);
$archivo=base64_decode($contenidoFichero);

if(strlen($macMaquina)<30)
{
	#Generamos el nombre de la tabla donde se almacena la respuesta y el nombre de la maquina en la tabla t_maquinas
	$maq=$_SERVER['REMOTE_ADDR'].'__'.$macMaquina;
	$tabla='t_'.str_replace(array('.'),array('_'),$_SERVER['REMOTE_ADDR']).'__'.$macMaquina;

	$result_a=mysql_query("SELECT COUNT(*) FROM t_maquinas WHERE maquina='".$maq."'");
	if ($row = mysql_fetch_array($result_a))
	{
		#Se comprueba que exista la máquina para aceptar el archivo
		if($row[0]>0)
		{
			#Almacenamos en la BBDD un mensaje indicando que se ha recibido el archivo correctamente
			$result=mysql_query("INSERT INTO ".$tabla." (comando, respuesta, finalizado, fecha, id_unico_comando, mostrado) VALUES ('" . $comando . "','Archivo recibido correctamente.',true,now(),".$id_unico_comando.",false) ",$conexion);

			$fp = fopen("descargas/".$nombre.$formato,'a');
			fwrite($fp, $archivo);
			fclose($fp);

			#Abrimos el fichero en modo de escritura 
			$fp2 = fopen("descargas/".$nombre."_signature.txt","w"); 
			#Escribimos la primera línea dentro de él 
			$cadena = "SHA1: ".$sha1; 
			fputs($fp2,$cadena); 
			#Escribimos la segunda línea de texto 
			$cadena = "  MD5: ".$md5; 
			fputs($fp2,$cadena); 
			#Cerramos el fichero 
			fclose($fp2);
		}
	}
}
include "cerrar-conexion-bbdd.php";
?>